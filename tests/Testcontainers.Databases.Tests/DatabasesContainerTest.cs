namespace Testcontainers.Databases;

public sealed class DatabaseContainersTest
{
    [Theory]
    [MemberData(nameof(DatabaseContainersTheoryData))]
    public void ImplementsIDatabaseContainerInterface(Type type)
    {
        Assert.True(type.IsAssignableTo(typeof(IDatabaseContainer)));
    }

    private static readonly HashSet<Type> NotDatabaseContainerTypes = new()
    {
        typeof(LocalStack.LocalStackContainer),
        typeof(WebDriver.WebDriverContainer),
    };

    private static bool IsDatabaseContainerType(Type containerType) => !NotDatabaseContainerTypes.Contains(containerType);

    public static IEnumerable<object[]> DatabaseContainersTheoryData
    {
        get
        {
            static bool HasGetConnectionStringMethod(Type type) => type.IsAssignableTo(typeof(IContainer)) && type.GetMethod("GetConnectionString") != null;
            var assembly = typeof(DatabaseContainersTest).Assembly;
            var dependencyContext = DependencyContext.Load(assembly) ?? throw new InvalidOperationException($"DependencyContext.Load({assembly}) returned null");
            return dependencyContext.RuntimeLibraries
                .Where(library => library.Name.StartsWith("Testcontainers."))
                .SelectMany(library => Assembly.Load(library.Name).GetExportedTypes().Where(HasGetConnectionStringMethod))
                .Where(IsDatabaseContainerType)
                .Select(type => new[] { type });
        }
    }
}