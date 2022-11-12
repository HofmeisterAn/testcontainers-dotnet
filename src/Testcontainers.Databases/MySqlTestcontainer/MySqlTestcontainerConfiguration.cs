namespace DotNet.Testcontainers.Configurations
{
  using System;
  using DotNet.Testcontainers.Builders;
  using JetBrains.Annotations;

  /// <inheritdoc cref="TestcontainerDatabaseConfiguration" />
  [PublicAPI]
  public class MySqlTestcontainerConfiguration : TestcontainerDatabaseConfiguration
  {
    private const string MySqlRootUsername = "root";

    private const string MySqlImage = "mysql:8.0.28";

    private const int MySqlPort = 3306;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlTestcontainerConfiguration" /> class.
    /// </summary>
    public MySqlTestcontainerConfiguration()
      : this(MySqlImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlTestcontainerConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    public MySqlTestcontainerConfiguration(string image)
      : base(image, MySqlPort)
    {
      this.Environments["MYSQL_ALLOW_EMPTY_PASSWORD"] = "yes";
    }

    /// <inheritdoc />
    public override string Database
    {
      get => this.Environments["MYSQL_DATABASE"];
      set => this.Environments["MYSQL_DATABASE"] = value;
    }

    /// <inheritdoc />
    public override string Username
    {
      get
      {
        return this.Environments.TryGetValue("MYSQL_USER", out var username) ? username : MySqlRootUsername;
      }

      set
      {
        if (MySqlRootUsername.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
          return;
        }

        this.Environments["MYSQL_USER"] = value;
      }
    }

    /// <inheritdoc />
    public override string Password
    {
      get => this.Environments["MYSQL_PASSWORD"];
      set
      {
        this.Environments["MYSQL_PASSWORD"] = value;
        this.Environments["MYSQL_ROOT_PASSWORD"] = value;
      }
    }

    /// <inheritdoc />
    public override IWaitForContainerOS WaitStrategy => Wait.ForUnixContainer()
      .UntilCommandIsCompleted("mysql", "--host=localhost", $"--port={this.DefaultPort}", $"--user={this.Username}", $"--password={this.Password}", "--protocol=TCP", "--execute=SHOW DATABASES");
  }
}
