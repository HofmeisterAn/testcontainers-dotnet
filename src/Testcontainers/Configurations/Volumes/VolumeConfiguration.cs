﻿namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IVolumeConfiguration" />
  [PublicAPI]
  internal sealed class VolumeConfiguration : ResourceConfiguration, IVolumeConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeConfiguration" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public VolumeConfiguration(
      string name = null)
    {
      this.Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public VolumeConfiguration(IResourceConfiguration resourceConfiguration)
      : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public VolumeConfiguration(IVolumeConfiguration resourceConfiguration)
      : this(new VolumeConfiguration(), resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public VolumeConfiguration(IVolumeConfiguration oldValue, IVolumeConfiguration newValue)
      : base(oldValue, newValue)
    {
      this.Name = BuildConfiguration.Combine(oldValue.Name, newValue.Name);
    }

    /// <inheritdoc />
    public string Name { get; }
  }
}
