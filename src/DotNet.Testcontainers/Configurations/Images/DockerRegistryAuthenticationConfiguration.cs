namespace DotNet.Testcontainers.Configurations
{
  /// <inheritdoc cref="IDockerRegistryAuthenticationConfiguration" />
  /// <remarks>
  /// In the future, we will replace this class. Instead, we will use the local Docker credentials.
  /// </remarks>
  internal readonly struct DockerRegistryAuthenticationConfiguration : IDockerRegistryAuthenticationConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationConfiguration" /> struct.
    /// </summary>
    /// <param name="registryEndpoint">The Docker registry endpoint.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    public DockerRegistryAuthenticationConfiguration(
      string registryEndpoint = null,
      string username = null,
      string password = null,
      string identityToken = null)
    {
      this.RegistryEndpoint = registryEndpoint;
      this.Username = username;
      this.Password = password;
      this.IdentityToken = identityToken;
    }

    /// <inheritdoc />
    public string RegistryEndpoint { get; }

    /// <inheritdoc />
    public string Username { get; }

    /// <inheritdoc />
    public string Password { get; }

    /// <inheritdoc />
    public string IdentityToken { get; }
  }
}
