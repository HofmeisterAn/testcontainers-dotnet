namespace Testcontainers.Azurite;

public abstract class AzuriteContainerConnectionStringHttpsPemTest : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithPemCertificate(Certificate.PEMPath, Certificate.PEMKeyPath)
        .Build();

    public Task InitializeAsync() => _azuriteContainer.StartAsync();

    public Task DisposeAsync() => _azuriteContainer.DisposeAsync().AsTask();

    private static bool HasError<TResponseEntity>(NullableResponse<TResponseEntity> response)
    {
        using (var rawResponse = response.GetRawResponse())
        {
            return rawResponse.IsError;
        }
    }

    public sealed class BlobService : AzuriteContainerConnectionStringHttpsPemTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new BlobServiceClient(_azuriteContainer.GetConnectionString());

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class QueueService : AzuriteContainerConnectionStringHttpsPemTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new QueueServiceClient(_azuriteContainer.GetConnectionString());

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }

    public sealed class TableService : AzuriteContainerConnectionStringHttpsPemTest
    {
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task EstablishesConnection()
        {
            // Give
            var client = new TableServiceClient(_azuriteContainer.GetConnectionString());

            // When
            var properties = await client.GetPropertiesAsync()
                .ConfigureAwait(false);

            // Then
            Assert.False(HasError(properties));
        }
    }
}