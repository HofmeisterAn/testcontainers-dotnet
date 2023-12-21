namespace Testcontainers.FirebirdSql;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class FirebirdSqlConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlConfiguration" /> class.
    /// </summary>
    /// <param name="database">The FirebirdSql database.</param>
    /// <param name="username">The FirebirdSql username.</param>
    /// <param name="password">The FirebirdSql password.</param>
    public FirebirdSqlConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirebirdSqlConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirebirdSqlConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirebirdSqlConfiguration(FirebirdSqlConfiguration resourceConfiguration)
        : this(new FirebirdSqlConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public FirebirdSqlConfiguration(FirebirdSqlConfiguration oldValue, FirebirdSqlConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the FirebirdSql database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the FirebirdSql username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the FirebirdSql password.
    /// </summary>
    public string Password { get; }
}