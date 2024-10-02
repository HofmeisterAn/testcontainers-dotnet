namespace Testcontainers.Neo4j;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class Neo4jBuilder : ContainerBuilder<Neo4jBuilder, Neo4jContainer, Neo4jConfiguration>
{
    public const string Neo4jImage = "neo4j:5.4";

    public const string Neo4jEnterpriseImage = "neo4j:5.4-enterprise";

    public const ushort Neo4jHttpPort = 7474;

    public const ushort Neo4jBoltPort = 7687;

    private const string AcceptLicenseAgreementEnvVar = "NEO4J_ACCEPT_LICENSE_AGREEMENT";

    private const string AcceptLicenseAgreement = "yes";

    private const string DeclineLicenseAgreement = "no";

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    public Neo4jBuilder()
        : this(new Neo4jConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private Neo4jBuilder(Neo4jConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override Neo4jConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the image to the Neo4j Enterprise Edition.
    /// </summary>
    /// <remarks>
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Neo4j Enterprise Edition <see href="https://neo4j.com/docs/operations-manual/current/docker/introduction/#_neo4j_editions">license</see> is accepted.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Neo4j Enterprise Edition license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="Neo4jBuilder" />.</returns>
    public Neo4jBuilder WithEnterpriseEdition(bool acceptLicenseAgreement)
    {
        return WithImage(Neo4jEnterpriseImage).WithEnvironment(AcceptLicenseAgreementEnvVar, acceptLicenseAgreement ? AcceptLicenseAgreement : DeclineLicenseAgreement);
    }

    /// <inheritdoc />
    public override Neo4jContainer Build()
    {
        Validate();
        return new Neo4jContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        var message = "The image '" + DockerResourceConfiguration.Image.FullName + "' requires you to accept a license agreement.";

        base.Validate();

        Predicate<Neo4jConfiguration> licenseAgreementNotAccepted = value => value.Image.Tag != null && value.Image.Tag.Contains("enterprise")
            && (!value.Environments.TryGetValue(AcceptLicenseAgreementEnvVar, out var licenseAgreementValue) || !AcceptLicenseAgreement.Equals(licenseAgreementValue, StringComparison.Ordinal));

        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Image))
            .ThrowIf(argument => licenseAgreementNotAccepted(argument.Value), argument => throw new ArgumentException(message, argument.Name));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Init()
    {
        return base.Init()
            .WithImage(Neo4jImage)
            .WithPortBinding(Neo4jHttpPort, true)
            .WithPortBinding(Neo4jBoltPort, true)
            .WithEnvironment("NEO4J_AUTH", "none")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(Neo4jHttpPort)));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Merge(Neo4jConfiguration oldValue, Neo4jConfiguration newValue)
    {
        return new Neo4jBuilder(new Neo4jConfiguration(oldValue, newValue));
    }
}