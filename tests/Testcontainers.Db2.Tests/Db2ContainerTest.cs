using IBM.Data.Db2;
using Testcontainers.Tests;

namespace Testcontainers.Db2;

public sealed class Db2ContainerTest : IAsyncLifetime
{
  private readonly Db2Container _db2Container = new Db2Builder().Build();

  public Task InitializeAsync()
  {
    return _db2Container.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _db2Container.DisposeAsync().AsTask();
  }

  [SkipOnLinuxEngine]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Windows))]
  public void ConnectionStateReturnsOpen()
  {
    // Given
    using DbConnection connection = new DB2Connection(_db2Container.GetConnectionString());

    // When
    connection.Open();

    // Then
    Assert.Equal(ConnectionState.Open, connection.State);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task ExecScriptReturnsSuccessful()
  {
    // Given
    const string scriptContent = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";

    // When
    var execResult = await _db2Container.ExecScriptAsync(scriptContent);

    // Then
    Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    Assert.Empty(execResult.Stderr);
  }
}
