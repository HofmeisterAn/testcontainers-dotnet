namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public sealed class DockerTlsFixture : ProtectDockerDaemonSocket
  {
    public const string DockerVersion = "20.10.18";
    public DockerTlsFixture()
      : base(new ContainerBuilder()
        .WithCommand("--tlsverify=false"), DockerVersion)
    {
    }

    public override IList<string> CustomProperties
    {
      get
      {
        var customProperties = base.CustomProperties;
        customProperties.Add("docker.tls=true");
        customProperties.Add("docker.tls.verify=false");
        return customProperties;
      }
    }
  }
}
