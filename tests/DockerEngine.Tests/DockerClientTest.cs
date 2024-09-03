﻿namespace DockerEngine;

public sealed class DockerClientTest : IAsyncLifetime
{
    private const string DockerHost = "0.0.0.0";

    private const ushort DockerPort = 2375;

    private readonly IContainer _dockerContainer = new ContainerBuilder()
        .WithImage("docker:24.0.5-dind")
        .WithEntrypoint("dockerd")
        .WithCommand("--host", "tcp://" + DockerHost + ":" + DockerPort, "--debug")
        .WithPortBinding(DockerPort, true)
        .WithPrivileged(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("API listen on \\[::\\]:" + DockerPort))
        .Build();

    public System.Threading.Tasks.Task InitializeAsync()
    {
        // return System.Threading.Tasks.Task.CompletedTask;
        return _dockerContainer.StartAsync();
    }

    public System.Threading.Tasks.Task DisposeAsync()
    {
        // return System.Threading.Tasks.Task.CompletedTask;
        return _dockerContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async System.Threading.Tasks.Task EstablishesConnection()
    {
        // Given
        const string repository = "alpine";

        const string tag = "latest";

        // TODO: Add support for additional schemes such as TCP, SSH, Unix, and Named Pipes (npipe) daemon socket.
        var dockerClient = new DockerClient(new UriBuilder(Uri.UriSchemeHttp, _dockerContainer.Hostname, _dockerContainer.GetMappedPublicPort(DockerPort)).ToString(), new HttpClient());

        // When
        // TODO: Consider creating request and response objects instead of using a lengthy list of arguments (avoid name clash with System.Threading.Tasks.Task etc.).
        await dockerClient.ImageCreateAsync(repository, null, null, tag, null, string.Empty, null, null, null);

        // TODO: Somehow, the HTTP request terminates too early, and the Docker image cannot be used or is not available right away. The container creation operation returns: no such image.
        // await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(3));

        // TODO: Try to avoid wrapping the actual content within an additional (unnecessary) body type, like `Body : ContainerConfig`.
        var response = await dockerClient.ContainerCreateAsync(null, null, new Body { Image = repository + ":" + tag });

        // Then
        Assert.Equal(64, response.Id.Length);
    }

    [Theory]
    [InlineData("tcp://127.0.0.1:2375")]
    [InlineData("npipe://./pipe/docker_engine")]
    [InlineData("unix:///var/run/docker.sock")]
    public async System.Threading.Tasks.Task CreatesDockerClient(string endpoint)
    {
        // TODO: Fix the '$endpoint' scheme is not supported.

        // Given
        using var httpClient = new HttpClient(HttpMessageHandlerFactory.GetHttpMessageHandler(new Uri(endpoint)));

        var dockerClient = new DockerClient(endpoint, httpClient);

        // When
        var systemInfo = await dockerClient.SystemInfoAsync();

        // Then
        Assert.NotEmpty(systemInfo.ID);
    }
}