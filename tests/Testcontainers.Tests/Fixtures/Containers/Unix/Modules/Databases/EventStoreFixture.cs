﻿namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using EventStore.Client;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class EventStoreFixture : DatabaseFixture<EventStoreTestcontainer, EventStoreClient>
  {
    private readonly TestcontainerDatabaseConfiguration configuration = new EventStoreTestcontainerConfiguration { Username = "admin", Password = "changeit" };

    public EventStoreFixture()
    {
      this.Container = new TestcontainersBuilder<EventStoreTestcontainer>()
        .WithDatabase(this.configuration)
        .Build();
    }

    public override async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      var settings = EventStoreClientSettings.Create(this.Container.ConnectionString);
      this.Connection = new EventStoreClient(settings);
    }

    public override async Task DisposeAsync()
    {
      this.Connection.Dispose();

      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.configuration.Dispose();
    }
  }
}
