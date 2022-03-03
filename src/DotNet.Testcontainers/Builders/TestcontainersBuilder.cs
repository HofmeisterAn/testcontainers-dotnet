namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Network;
  using DotNet.Testcontainers.Volumes;
  using JetBrains.Annotations;

  /// <summary>
  /// This class represents the fluent Testcontainer builder. Each change creates a new instance of <see cref="ITestcontainersBuilder{TDockerContainer}" />.
  /// With this behaviour we can reuse previous configured configurations and create similar Testcontainer with only little effort.
  /// </summary>
  /// <example>
  /// <code>
  ///   var builder = new builder&lt;TestcontainersContainer&gt;()
  ///     .WithName(&quot;nginx&quot;)
  ///     .WithImage(&quot;nginx&quot;)
  ///     .WithEntrypoint(&quot;...&quot;)
  ///     .WithCommand(&quot;...&quot;);
  ///   <br />
  ///   var http = builder
  ///     .WithPortBinding(80, 08)
  ///     .Build();
  ///   <br />
  ///   var https = builder
  ///     .WithPortBinding(443, 443)
  ///     .Build();
  /// </code>
  /// </example>
  /// <typeparam name="TDockerContainer">Type of <see cref="ITestcontainersContainer" />.</typeparam>
  [PublicAPI]
  public sealed class TestcontainersBuilder<TDockerContainer> : ITestcontainersBuilder<TDockerContainer>
    where TDockerContainer : ITestcontainersContainer
  {
    private readonly ITestcontainersConfiguration configuration;

    private readonly Action<TDockerContainer> moduleConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersBuilder{TDockerContainer}" /> class.
    /// </summary>
    public TestcontainersBuilder()
      : this(
        Apply(
          endpoint: TestcontainersSettings.OS.DockerApiEndpoint,
          dockerRegistryAuthConfig: default(DockerRegistryAuthenticationConfiguration),
          labels: DefaultLabels.Instance,
          outputConsumer: Consume.DoNotConsumeStdoutAndStderr(),
          waitStrategies: Wait.ForUnixContainer().Build(),
          startupCallback: (testcontainers, ct) => Task.CompletedTask,
          autoRemove: false,
          privileged: false),
        _ => { })
    {
    }

    private TestcontainersBuilder(
      ITestcontainersConfiguration configuration,
      Action<TDockerContainer> moduleConfiguration)
    {
      this.configuration = configuration;
      this.moduleConfiguration = moduleConfiguration;
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> ConfigureContainer(Action<TDockerContainer> moduleConfiguration)
    {
      return Build(this, Apply(), moduleConfiguration);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithImage(string image)
    {
      return this.WithImage(new DockerImage(image));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithImage(IDockerImage image)
    {
      return Build(this, Apply(image: image));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithName(string name)
    {
      return Build(this, Apply(name: name));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithHostname(string hostname)
    {
      return Build(this, Apply(hostname: hostname));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithWorkingDirectory(string workingDirectory)
    {
      return Build(this, Apply(workingDirectory: workingDirectory));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithEntrypoint(params string[] entrypoint)
    {
      return Build(this, Apply(entrypoint: entrypoint));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithCommand(params string[] command)
    {
      return Build(this, Apply(command: command));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithEnvironment(string name, string value)
    {
      var environments = new Dictionary<string, string> { { name, value } };
      return Build(this, Apply(environments: environments));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithEnvironment(IReadOnlyDictionary<string, string> environments)
    {
      return Build(this, Apply(environments: environments));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithLabel(string name, string value)
    {
      var labels = new Dictionary<string, string> { { name, value } };
      return Build(this, Apply(labels: labels));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithExposedPort(int port)
    {
      return this.WithExposedPort($"{port}");
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithExposedPort(string port)
    {
      var exposedPorts = new Dictionary<string, string> { { port, port } };
      return Build(this, Apply(exposedPorts: exposedPorts));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(int port, bool assignRandomHostPort = false)
    {
      return this.WithPortBinding($"{port}", assignRandomHostPort);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(int hostPort, int containerPort)
    {
      return this.WithPortBinding($"{hostPort}", $"{containerPort}");
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(string port, bool assignRandomHostPort = false)
    {
      var hostPort = assignRandomHostPort ? null : port;
      return this.WithPortBinding(hostPort, port);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithPortBinding(string hostPort, string containerPort)
    {
      var portBindings = new Dictionary<string, string> { { containerPort, hostPort } };
      return Build(this, Apply(portBindings: portBindings));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithMount(string source, string destination)
    {
      return this.WithBindMount(source, destination);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithMount(string source, string destination, AccessMode accessMode)
    {
      return this.WithBindMount(source, destination, accessMode);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithBindMount(string source, string destination)
    {
      return this.WithBindMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithBindMount(string source, string destination, AccessMode accessMode)
    {
      var mounts = new IMount[] { new BindMount(source, destination, accessMode) };
      return Build(this, Apply(mounts: mounts));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(string source, string destination)
    {
      return this.WithVolumeMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(string source, string destination, AccessMode accessMode)
    {
      return this.WithVolumeMount(new DockerVolume(source), destination, accessMode);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(IDockerVolume source, string destination)
    {
      return this.WithVolumeMount(source, destination, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithVolumeMount(IDockerVolume source, string destination, AccessMode accessMode)
    {
      var mounts = new IMount[] { new VolumeMount(source, destination, accessMode) };
      return Build(this, Apply(mounts: mounts));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithNetwork(string id, string name)
    {
      return this.WithNetwork(new DockerNetwork(id, name));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithNetwork(IDockerNetwork dockerNetwork)
    {
      var networks = new[] { dockerNetwork };
      return Build(this, Apply(networks: networks));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithCleanUp(bool cleanUp)
    {
      return this.WithResourceReaperSessionId(TestcontainersSettings.ResourceReaperEnabled && cleanUp ? ResourceReaper.DefaultSessionId : Guid.Empty);
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithAutoRemove(bool autoRemove)
    {
      return Build(this, Apply(autoRemove: autoRemove));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithPrivileged(bool privileged)
    {
      return Build(this, Apply(privileged: privileged));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithDockerEndpoint(string endpoint)
    {
      return Build(this, Apply(endpoint: new Uri(endpoint)));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithRegistryAuthentication(string registryEndpoint, string username, string password)
    {
      return Build(this, Apply(dockerRegistryAuthConfig: new DockerRegistryAuthenticationConfiguration(new Uri(registryEndpoint), username, password)));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithOutputConsumer(IOutputConsumer outputConsumer)
    {
      return Build(this, Apply(outputConsumer: outputConsumer));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithWaitStrategy(IWaitForContainerOS waitStrategy)
    {
      return Build(this, Apply(waitStrategies: waitStrategy.Build()));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithStartupCallback(Func<IRunningDockerContainer, CancellationToken, Task> startupCallback)
    {
      return Build(this, Apply(startupCallback: startupCallback));
    }

    /// <inheritdoc />
    public ITestcontainersBuilder<TDockerContainer> WithResourceReaperSessionId(Guid resourceReaperSessionId)
    {
      return this.WithLabel(ResourceReaper.ResourceReaperSessionLabel, resourceReaperSessionId.ToString("D"));
    }

    /// <inheritdoc />
    public TDockerContainer Build()
    {
      if (this.configuration.Image is null)
      {
        throw new ArgumentNullException(nameof(this.configuration.Image));
      }

#pragma warning disable S3011

      // Create container instance.
      var container = (TDockerContainer)Activator.CreateInstance(typeof(TDockerContainer), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { this.configuration, TestcontainersSettings.Logger }, null);

#pragma warning restore S3011

      // Apply specific container configuration.
      this.moduleConfiguration.Invoke(container);

      return container;
    }

#pragma warning disable S107

    private static ITestcontainersConfiguration Apply(
      Uri endpoint = null,
      IDockerRegistryAuthenticationConfiguration dockerRegistryAuthConfig = null,
      IDockerImage image = null,
      string name = null,
      string hostname = null,
      string workingDirectory = null,
      IEnumerable<string> entrypoint = null,
      IEnumerable<string> command = null,
      IReadOnlyDictionary<string, string> environments = null,
      IReadOnlyDictionary<string, string> labels = null,
      IReadOnlyDictionary<string, string> exposedPorts = null,
      IReadOnlyDictionary<string, string> portBindings = null,
      IEnumerable<IMount> mounts = null,
      IEnumerable<IDockerNetwork> networks = null,
      IOutputConsumer outputConsumer = null,
      IEnumerable<IWaitUntil> waitStrategies = null,
      Func<ITestcontainersContainer, CancellationToken, Task> startupCallback = null,
      bool? autoRemove = null,
      bool? privileged = null)
    {
      return new TestcontainersConfiguration(
        endpoint,
        dockerRegistryAuthConfig,
        image,
        name,
        hostname,
        workingDirectory,
        entrypoint,
        command,
        environments,
        labels,
        exposedPorts,
        portBindings,
        mounts,
        networks,
        outputConsumer,
        waitStrategies,
        startupCallback,
        autoRemove,
        privileged);
    }

#pragma warning restore S107

    private static ITestcontainersBuilder<TDockerContainer> Build(
      TestcontainersBuilder<TDockerContainer> previous,
      ITestcontainersConfiguration next,
      Action<TDockerContainer> moduleConfiguration = null)
    {
      var autoRemove = next.AutoRemove ?? previous.configuration.AutoRemove;
      var privileged = next.Privileged ?? previous.configuration.Privileged;
      var endpoint = BuildConfiguration.Combine(next.Endpoint, previous.configuration.Endpoint);
      var image = BuildConfiguration.Combine(next.Image, previous.configuration.Image);
      var name = BuildConfiguration.Combine(next.Name, previous.configuration.Name);
      var hostname = BuildConfiguration.Combine(next.Hostname, previous.configuration.Hostname);
      var workingDirectory = BuildConfiguration.Combine(next.WorkingDirectory, previous.configuration.WorkingDirectory);
      var entrypoint = BuildConfiguration.Combine(next.Entrypoint, previous.configuration.Entrypoint);
      var command = BuildConfiguration.Combine(next.Command, previous.configuration.Command);
      var environments = BuildConfiguration.Combine(next.Environments, previous.configuration.Environments);
      var labels = BuildConfiguration.Combine(next.Labels, previous.configuration.Labels);
      var exposedPorts = BuildConfiguration.Combine(next.ExposedPorts, previous.configuration.ExposedPorts);
      var portBindings = BuildConfiguration.Combine(next.PortBindings, previous.configuration.PortBindings);
      var mounts = BuildConfiguration.Combine(next.Mounts, previous.configuration.Mounts);
      var networks = BuildConfiguration.Combine(next.Networks, previous.configuration.Networks);

      var authConfig = new[] { next.DockerRegistryAuthConfig, previous.configuration.DockerRegistryAuthConfig }.First(config => config != null);
      var outputConsumer = new[] { next.OutputConsumer, previous.configuration.OutputConsumer }.First(config => config != null);
      var waitStrategies = new[] { next.WaitStrategies, previous.configuration.WaitStrategies }.First(config => config != null);
      var startupCallback = new[] { next.StartupCallback, previous.configuration.StartupCallback }.First(config => config != null);

      var mergedConfiguration = Apply(
        endpoint,
        authConfig,
        image,
        name,
        hostname,
        workingDirectory,
        entrypoint,
        command,
        environments,
        labels,
        exposedPorts,
        portBindings,
        mounts,
        networks,
        outputConsumer,
        waitStrategies,
        startupCallback,
        autoRemove,
        privileged);

      return new TestcontainersBuilder<TDockerContainer>(mergedConfiguration, moduleConfiguration ?? previous.moduleConfiguration);
    }
  }
}
