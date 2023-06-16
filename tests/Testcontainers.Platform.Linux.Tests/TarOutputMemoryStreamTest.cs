namespace Testcontainers.Tests;

public abstract class TarOutputMemoryStreamTest
{
    private const string TargetDirectoryPath = "/tmp";

    private readonly TarOutputMemoryStream _tarOutputMemoryStream = new TarOutputMemoryStream(TargetDirectoryPath);

    private readonly FileInfo _testFile = new FileInfo(Path.Combine(TestSession.TempDirectoryPath, Path.GetRandomFileName()));

    protected TarOutputMemoryStreamTest()
    {
        using var fileStream = _testFile.Create();
        fileStream.WriteByte(13);
    }

    [Fact]
    public void TarFileContainsTestFile()
    {
        // Given
        IList<string> actual = new List<string>();

        _tarOutputMemoryStream.Close();
        _tarOutputMemoryStream.Seek(0, SeekOrigin.Begin);

        // When
        using var tarIn = TarArchive.CreateInputTarArchive(_tarOutputMemoryStream, Encoding.Default);
        tarIn.ProgressMessageEvent += (_, entry, _) => actual.Add(entry.Name);
        tarIn.ListContents();

        // Then
        Assert.Contains(actual, file => file.EndsWith(_testFile.Name));
    }

    [UsedImplicitly]
    public sealed class FromResourceMapping : TarOutputMemoryStreamTest, IResourceMapping, IAsyncLifetime, IDisposable
    {
        public MountType Type
            => MountType.Bind;

        public AccessMode AccessMode
            => AccessMode.ReadOnly;

        public string Source
            => string.Empty;

        public string Target
            => string.Join("/", TargetDirectoryPath, _testFile.Name);

        public UnixFileMode FileMode
            => Unix.FileMode644;

        public Task InitializeAsync()
        {
            return _tarOutputMemoryStream.AddAsync(this);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tarOutputMemoryStream.Dispose();
        }

        public Task CreateAsync(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task<byte[]> GetAllBytesAsync(CancellationToken ct = default)
        {
            return File.ReadAllBytesAsync(_testFile.FullName, ct);
        }
    }

    [UsedImplicitly]
    public sealed class FromFile : TarOutputMemoryStreamTest, IAsyncLifetime, IDisposable
    {
        public Task InitializeAsync()
        {
            return _tarOutputMemoryStream.AddAsync(_testFile, Unix.FileMode644);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tarOutputMemoryStream.Dispose();
        }
    }

    [UsedImplicitly]
    public sealed class FromDirectory : TarOutputMemoryStreamTest, IAsyncLifetime, IDisposable
    {
        public Task InitializeAsync()
        {
            return _tarOutputMemoryStream.AddAsync(_testFile.Directory, true, Unix.FileMode644);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _tarOutputMemoryStream.Dispose();
        }
    }

    public sealed class UnixFileModeTest
    {
        [Theory]
        [InlineData(Unix.FileMode644, "644")]
        [InlineData(Unix.FileMode666, "666")]
        [InlineData(Unix.FileMode700, "700")]
        [InlineData(Unix.FileMode755, "755")]
        [InlineData(Unix.FileMode777, "777")]
        public void UnixFileModeResolvesToPosixFilePermission(UnixFileMode fileMode, string posixFilePermission)
        {
            Assert.Equal(Convert.ToInt32(posixFilePermission, 8), Convert.ToInt32(fileMode));
        }
    }
}