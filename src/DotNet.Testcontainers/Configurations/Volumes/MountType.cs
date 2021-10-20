namespace DotNet.Testcontainers.Configurations
{
  /// <summary>
  /// The mount type.
  /// </summary>
  public readonly struct MountType
  {
    /// <summary>
    /// The 'bind' mount type.
    /// </summary>
    public static readonly MountType Bind = new MountType("bind");

    /// <summary>
    /// The 'volume' mount type.
    /// </summary>
    public static readonly MountType Volume = new MountType("volume");

    /// <summary>
    /// Initializes a new instance of the <see cref="MountType" /> struct.
    /// </summary>
    /// <param name="type">The mount type.</param>
    private MountType(string type)
    {
      this.Type = type;
    }

    /// <summary>
    /// Gets the mount type.
    /// </summary>
    public string Type { get; }
  }
}
