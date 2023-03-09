namespace Testcontainers.WebDriver;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class WebDriverBuilder : ContainerBuilder<WebDriverBuilder, WebDriverContainer, WebDriverConfiguration>
{
    public const string WebDriverNetworkAlias = "standalone-container";

    public const string FFmpegNetworkAlias = "ffmpeg-container";

    public const string FFmpegImage = "selenium/video:ffmpeg-4.3.1-20230306";

    public const ushort WebDriverPort = 4444;

    public const ushort VncServerPort = 5900;

    public static readonly string VideoFilePath = string.Join("/", string.Empty, "videos", "video.mp4");

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    public WebDriverBuilder()
        : this(new WebDriverConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private WebDriverBuilder(WebDriverConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override WebDriverConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Web Driver browser configuration.
    /// </summary>
    /// <remarks>
    /// https://www.selenium.dev/documentation/webdriver/browsers/
    /// </remarks>
    /// <param name="webDriverBrowser">The Web Driver browser configuration.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithBrowser(WebDriverBrowser webDriverBrowser)
    {
        return WithImage(webDriverBrowser.Image);
    }

    /// <summary>
    /// Sets Selenium Grid container configurations.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#se_opts-selenium-configuration-options.
    /// </remarks>
    /// <param name="options">A list of Selenium Grid container configurations.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithConfigurationOptions(IReadOnlyDictionary<string, string> options)
    {
        return WithEnvironment("SE_OPTS",
            string.Join(" ", options.Select(option => string.Join("=", option.Key, option.Value))));
    }

    /// <summary>
    /// Sets JVM configurations.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#se_java_opts-java-environment-options.
    /// </remarks>
    /// <param name="javaOptions">A list of JVM configurations.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithJavaEnvironmentOptions(IEnumerable<string> javaOptions)
    {
        return WithEnvironment("SE_JAVA_OPTS", string.Join(" ", javaOptions));
    }

    /// <summary>
    /// Sets the screen resolution.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#setting-screen-resolution.
    /// </remarks>
    /// <param name="width">The screen width.</param>
    /// <param name="height">The screen height.</param>
    /// <param name="depth">The screen depth.</param>
    /// <param name="dpi">The screen dpi.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SettingScreenResolution(int width = 1020, int height = 1360, int depth = 24, int dpi = 96)
    {
        IDictionary<string, string> screenResolution = new Dictionary<string, string>();
        screenResolution.Add("SE_SCREEN_WIDTH", width.ToString());
        screenResolution.Add("SE_SCREEN_HEIGHT", height.ToString());
        screenResolution.Add("SE_SCREEN_DEPTH", depth.ToString());
        screenResolution.Add("SE_SCREEN_DPI", dpi.ToString());
        return WithEnvironment(new ReadOnlyDictionary<string, string>(screenResolution));
    }

    /// <summary>
    /// Sets the session timeout.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#grid-url-and-session-timeout.
    /// </remarks>
    /// <param name="sessionTimeout">The session timeout.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetSessionTimeout(TimeSpan sessionTimeout = default)
    {
        return WithEnvironment("SE_NODE_SESSION_TIMEOUT", sessionTimeout.TotalSeconds.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Sets the time zone.
    /// </summary>
    /// <remarks>
    /// Time Zone Database name (IANA).
    /// </remarks>
    /// <param name="timeZone">The time zone.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetTimeZone(string timeZone)
    {
        return WithEnvironment("TZ", timeZone);
    }

    /// <summary>
    /// Overrides the Selenium Grid configurations with the TOML file.
    /// </summary>
    /// <remarks>
    /// https://www.selenium.dev/documentation/grid/configuration/toml_options/.
    /// </remarks>
    /// <param name="configTomlFilePath">The TOML file.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithConfigurationFromTomlFile(string configTomlFilePath)
    {
        return WithResourceMapping(configTomlFilePath, "/opt/bin/config.toml");
    }

    /// <summary>
    /// Enables the video recording.
    /// </summary>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithRecording()
    {
        var ffmpegContainer = new ContainerBuilder()
            .WithImage(FFmpegImage)
            .WithNetwork(DockerResourceConfiguration.Networks.Single())
            .WithNetworkAliases(FFmpegNetworkAlias)
            .WithEnvironment("FILE_NAME", Path.GetFileName(VideoFilePath))
            .WithEnvironment("DISPLAY_CONTAINER_NAME", WebDriverNetworkAlias)
            .Build();

        // TODO: Pass the depended container (Docker resource) to the builder and resolve the dependency graph internal (not by an individual property).
        return Merge(DockerResourceConfiguration, new WebDriverConfiguration(ffmpegContainer: ffmpegContainer));
    }

    /// <inheritdoc />
    public override WebDriverContainer Build()
    {
        Validate();
        return new WebDriverContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Init()
    {
        return base.Init()
            .WithBrowser(WebDriverBrowser.Chrome)
            .WithNetwork(new NetworkBuilder().Build())
            .WithNetworkAliases(WebDriverNetworkAlias)
            .WithPortBinding(WebDriverPort, true)
            .WithPortBinding(VncServerPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request
                => request.ForPath("/wd/hub/status").ForPort(WebDriverPort).ForResponseMessageMatching(IsGridReadyAsync)));
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WebDriverConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WebDriverConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Merge(WebDriverConfiguration oldValue, WebDriverConfiguration newValue)
    {
        return new WebDriverBuilder(new WebDriverConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Determines whether the Selenium Grid is up and ready to receive requests.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#waiting-for-the-grid-to-be-ready.
    /// </remarks>
    /// <param name="response">The HTTP response that contains the Selenium Grid information.</param>
    /// <returns>A value indicating whether the Selenium Grid is ready.</returns>
    private async Task<bool> IsGridReadyAsync(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        try
        {
            return JsonDocument.Parse(jsonString)
                .RootElement
                .GetProperty("value")
                .GetProperty("ready")
                .GetBoolean();
        }
        catch
        {
            return false;
        }
    }
}