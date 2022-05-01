namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Runtime.Serialization;

  [Serializable]
  public sealed class ResourceReaperException : Exception
  {
    public ResourceReaperException(string message, ResourceReaperDiagnostics diagnostics)
      : base(string.Join(Environment.NewLine, message, diagnostics.ToString()))
    {
    }

    private ResourceReaperException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
