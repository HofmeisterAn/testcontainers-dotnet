namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System.Data.Common;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;

  public class CosmosDbFixture : DatabaseFixture<CosmosDbTestcontainer, DbConnection>
  {
    public CosmosDbFixture()
      : this(new CosmosDbTestcontainerConfiguration())
    {
    }

    private CosmosDbFixture(CosmosDbTestcontainerConfiguration configuration)
    {
      this.Configuration = configuration;
      this.Rebuild(configuration);
    }

    private void Rebuild(CosmosDbTestcontainerConfiguration configuration)
    {
      this.Container = new TestcontainersBuilder<CosmosDbTestcontainer>()
        .WithCosmosDb(configuration)
        .Build();
    }

    public CosmosDbTestcontainerConfiguration Configuration { get; set; }

    public override async Task InitializeAsync()
    {
      const int maxRetries = 5;
      async Task Restart()
      {
        await this.Container.StartAsync();
        await Task.WhenAny(this.Container.StartAsync(), Task.Delay(5 * 60 * 1000));
      }

      await Restart();
      var outputMessage = this.ReadOutputMessage();
      var retries = 0;
      while (!outputMessage.Contains("Started") && retries++ < maxRetries)
      {
        await this.DisposeAsync();
        this.Rebuild(this.Configuration);
        await Restart();
        outputMessage = this.ReadOutputMessage();
      }
    }

    private string ReadOutputMessage()
    {
      var stdout = this.Configuration.OutputConsumer.Stdout;
      stdout.Position = 0;
      return new StreamReader(stdout).ReadToEnd();
    }

    public override async Task DisposeAsync()
    {
      if (this.Connection != null && this.Connection.State != System.Data.ConnectionState.Closed)
      {
        this.Connection.Dispose();
      }

      await this.Container.DisposeAsync()
        .ConfigureAwait(false);
    }

    public override void Dispose()
    {
      this.Configuration.Dispose();
    }
  }
}
