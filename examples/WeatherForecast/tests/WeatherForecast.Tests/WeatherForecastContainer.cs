﻿using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using JetBrains.Annotations;
using Testcontainers.SqlEdge;
using Xunit;

namespace WeatherForecast.Tests;

[UsedImplicitly]
public sealed class WeatherForecastContainer : HttpClient, IAsyncLifetime
{
  private static readonly X509Certificate Certificate = new X509Certificate2(WeatherForecastImage.CertificateFilePath, WeatherForecastImage.CertificatePassword);

  private static readonly WeatherForecastImage Image = new();

  private readonly INetwork _weatherForecastNetwork;

  private readonly IContainer _sqlEdgeContainer;

  private readonly IContainer _weatherForecastContainer;

  public WeatherForecastContainer()
    : base(new HttpClientHandler
    {
      // Trust the development certificate.
      ServerCertificateCustomValidationCallback = (_, certificate, _, _) => Certificate.Equals(certificate)
    })
  {
    const string weatherForecastStorage = "weatherForecastStorage";

    const string connectionString = $"server={weatherForecastStorage};user id={SqlEdgeBuilder.DefaultUsername};password={SqlEdgeBuilder.DefaultPassword};database={SqlEdgeBuilder.DefaultDatabase}";

    _weatherForecastNetwork = new NetworkBuilder()
      .Build();

    _sqlEdgeContainer = new SqlEdgeBuilder()
      .WithNetwork(_weatherForecastNetwork)
      .WithNetworkAliases(weatherForecastStorage)
      .Build();

    _weatherForecastContainer = new ContainerBuilder()
      .WithImage(Image)
      .WithNetwork(_weatherForecastNetwork)
      .WithPortBinding(WeatherForecastImage.HttpsPort, true)
      .WithEnvironment("ASPNETCORE_URLS", "https://+")
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Path", WeatherForecastImage.CertificateFilePath)
      .WithEnvironment("ASPNETCORE_Kestrel__Certificates__Default__Password", WeatherForecastImage.CertificatePassword)
      .WithEnvironment("ConnectionStrings__DefaultConnection", connectionString)
      .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(WeatherForecastImage.HttpsPort))
      .Build();
  }

  public async Task InitializeAsync()
  {
    // It is not necessary to clean up resources immediately (still good practice). The Resource Reaper will take care of orphaned resources.
    await Image.InitializeAsync()
      .ConfigureAwait(false);

    await _weatherForecastNetwork.CreateAsync()
      .ConfigureAwait(false);

    await _sqlEdgeContainer.StartAsync()
      .ConfigureAwait(false);

    await _weatherForecastContainer.StartAsync()
      .ConfigureAwait(false);
  }

  public async Task DisposeAsync()
  {
    await Image.DisposeAsync()
      .ConfigureAwait(false);

    await _weatherForecastContainer.DisposeAsync()
      .ConfigureAwait(false);

    await _sqlEdgeContainer.DisposeAsync()
      .ConfigureAwait(false);

    await _weatherForecastNetwork.DeleteAsync()
      .ConfigureAwait(false);
  }

  public void SetBaseAddress()
  {
    try
    {
      var uriBuilder = new UriBuilder("https", _weatherForecastContainer.Hostname, _weatherForecastContainer.GetMappedPublicPort(WeatherForecastImage.HttpsPort));
      BaseAddress = uriBuilder.Uri;
    }
    catch
    {
      // Set the base address only once.
    }
  }
}
