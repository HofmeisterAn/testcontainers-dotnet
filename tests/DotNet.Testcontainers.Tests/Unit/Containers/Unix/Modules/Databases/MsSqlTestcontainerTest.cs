namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Data;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class MsSqlTestcontainerTest : IClassFixture<MsSqlFixture>
  {
    private readonly MsSqlFixture msSqlFixture;

    public MsSqlTestcontainerTest(MsSqlFixture msSqlFixture)
    {
      this.msSqlFixture = msSqlFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.msSqlFixture.Connection;

      // When
      await connection.OpenAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void CannotSetDatabase()
    {
      var mssql = new MsSqlTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => mssql.Database = string.Empty);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var mssql = new MsSqlTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => mssql.Username = string.Empty);
    }

    [Fact]
    public async Task ExecScriptInRunningContainer()
    {
      // Given
      const string script = @"
        CREATE DATABASE testcontainers;
        GO
        USE testcontainers;
        GO
        CREATE TABLE MyTable (
        id INT,
        name VARCHAR(30) NOT NULL
        );
        GO
        INSERT INTO MyTable (id, name) VALUES (1, 'MyName');
        SELECT * FROM MyTable;
        ";

      // When
      var results = await this.msSqlFixture.Container.ExecScriptAsync(script);

      // Then
      Assert.Contains("MyName", results.Stdout);
    }

    [Fact]
    public async Task ThrowErrorInRunningContainerWithInvalidScript()
    {
      // Given
      const string script = "invalid SQL command";

      // When
      var results = await this.msSqlFixture.Container.ExecScriptAsync(script);

      // Then
      Assert.NotEqual(0, results.ExitCode);
      Assert.NotEqual(string.Empty, results.Stderr);
    }
  }
}
