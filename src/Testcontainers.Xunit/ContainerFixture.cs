namespace Testcontainers.Xunit;

/// <summary>
/// Fixture for sharing a container instance across multiple tests in a single class.
/// See <a href="https://xunit.net/docs/shared-context">Shared Context between Tests</a> from xUnit.net documentation for more information about fixtures.
/// A logger is automatically configured to write diagnostic messages to xUnit's <see cref="IMessageSink" />.
/// </summary>
/// <typeparam name="TBuilderEntity">The builder entity.</typeparam>
/// <typeparam name="TContainerEntity">The container entity.</typeparam>
[PublicAPI]
public class ContainerFixture<TBuilderEntity, TContainerEntity>(IMessageSink messageSink) : ContainerLifetime<TBuilderEntity, TContainerEntity>
    where TBuilderEntity : IContainerBuilder<TBuilderEntity, TContainerEntity>, new()
    where TContainerEntity : IContainer
{
    /// <summary>
    /// The message sink used for reporting diagnostic messages.
    /// </summary>
    protected IMessageSink MessageSink { get; } = messageSink;

    protected override ILogger Logger { get; } = new XunitLoggerProvider(messageSink).CreateLogger("testcontainers.org");
}