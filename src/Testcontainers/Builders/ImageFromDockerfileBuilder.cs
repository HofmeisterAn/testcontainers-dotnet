﻿namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker image builder.
  /// </summary>
  /// <example>
  ///   The default configuration is equivalent to:
  ///   <code>
  ///   _ = new ImageFromDockerfileBuilder()
  ///     .WithDockerEndpoint(TestcontainersSettings.OS.DockerEndpointAuthConfig)
  ///     .WithLabel(DefaultLabels.Instance)
  ///     .WithCleanUp(true)
  ///     .WithDockerfile("Dockerfile")
  ///     .WithDockerfileDirectory(Directory.GetCurrentDirectory())
  ///     .WithName(new DockerImage(string.Empty, "testcontainers", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)))
  ///     .Build();
  ///   </code>
  /// </example>
  [PublicAPI]
  public class ImageFromDockerfileBuilder : AbstractBuilder<ImageFromDockerfileBuilder, IFutureDockerImage, IImageFromDockerfileConfiguration>, IImageFromDockerfileBuilder<ImageFromDockerfileBuilder>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    public ImageFromDockerfileBuilder()
      : this(new ImageFromDockerfileConfiguration())
    {
      this.DockerResourceConfiguration = this.Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private ImageFromDockerfileBuilder(IImageFromDockerfileConfiguration dockerResourceConfiguration)
      : base(dockerResourceConfiguration)
    {
      this.DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override IImageFromDockerfileConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithName(string name)
    {
      return this.WithName(new DockerImage(name));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithName(IImage name)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageFromDockerfileConfiguration(image: name));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDockerfile(string dockerfile)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageFromDockerfileConfiguration(dockerfile: dockerfile));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageFromDockerfileConfiguration(dockerfileDirectory: dockerfileDirectory));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDockerfileDirectory(CommonDirectoryPath commonDirectoryPath, string dockerfileDirectory)
    {
      var dockerfileDirectoryPath = Path.Combine(commonDirectoryPath.DirectoryPath, dockerfileDirectory);
      return this.Merge(this.DockerResourceConfiguration, new ImageFromDockerfileConfiguration(dockerfileDirectory: dockerfileDirectoryPath));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageFromDockerfileConfiguration(deleteIfExists: deleteIfExists));
    }

    /// <inheritdoc />
    public ImageFromDockerfileBuilder WithBuildArgument(string name, string value)
    {
      var buildArguments = new Dictionary<string, string> { { name, value } };
      return this.Merge(this.DockerResourceConfiguration, new ImageFromDockerfileConfiguration(buildArguments: buildArguments));
    }

    public ImageFromDockerfileBuilder WithName(IDockerImage image)
    {
      return this.WithName(new DockerImage(image));
    }

    /// <inheritdoc />
    public override IFutureDockerImage Build()
    {
      this.Validate();
      return new FutureDockerImage(this.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected sealed override ImageFromDockerfileBuilder Init()
    {
      return base.Init().WithDockerfile("Dockerfile").WithDockerfileDirectory(Directory.GetCurrentDirectory()).WithName(new DockerImage(string.Empty, "testcontainers", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture)));
    }

    /// <inheritdoc />
    protected override ImageFromDockerfileBuilder Clone(IResourceConfiguration resourceConfiguration)
    {
      return this.Merge(this.DockerResourceConfiguration, new ImageFromDockerfileConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ImageFromDockerfileBuilder Merge(IImageFromDockerfileConfiguration oldValue, IImageFromDockerfileConfiguration newValue)
    {
      return new ImageFromDockerfileBuilder(new ImageFromDockerfileConfiguration(oldValue, newValue));
    }
  }
}
