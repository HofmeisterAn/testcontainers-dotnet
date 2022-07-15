namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class TestcontainersImageTest
  {
    [Fact]
    public void ShouldThrowArgumentNullExceptionWhenInstantiateDockerImage()
    {
      Assert.Throws<ArgumentNullException>(() => new DockerImage((string)null));
      Assert.Throws<ArgumentNullException>(() => new DockerImage(null, null, null));
      Assert.Throws<ArgumentNullException>(() => new DockerImage("fedora", null, null));
      Assert.Throws<ArgumentNullException>(() => new DockerImage("fedora", "httpd", null));
    }

    [Fact]
    public void ShouldThrowArgumentExceptionWhenInstantiateDockerImage()
    {
      Assert.Throws<ArgumentException>(() => new DockerImage(string.Empty));
      Assert.Throws<ArgumentException>(() => new DockerImage(string.Empty, string.Empty, string.Empty));
    }

    [Theory]
    [ClassData(typeof(DockerImageFixture))]
    public void WhenImageNameGetsAssigned(DockerImageFixtureSerializable serializable, string fullName)
    {
      // Given
      var expected = serializable.Image;

      // When
      IDockerImage dockerImage = new DockerImage(fullName);

      // Then
      Assert.Equal(expected.Repository, dockerImage.Repository);
      Assert.Equal(expected.Name, dockerImage.Name);
      Assert.Equal(expected.Tag, dockerImage.Tag);
      Assert.Equal(expected.FullName, dockerImage.FullName);
    }

    [Fact]
    public void ShouldThrowArgumentExceptionIfImageNameHasUpperCaseCharacters()
    {
      Assert.Throws<ArgumentException>(() => new DockerImage("Abc"));
      Assert.Throws<ArgumentException>(() => new DockerImage("Abc:def"));
    }

    [Fact]
    public void ShouldNotThrowArgumentExceptionIfTagNameHasUpperCaseCharacters()
    {
      var exception = Record.Exception(() => new DockerImage("abc:DEF"));
      Assert.Null(exception);
    }
  }
}
