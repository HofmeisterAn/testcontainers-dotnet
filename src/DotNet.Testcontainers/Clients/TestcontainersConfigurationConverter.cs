namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Network;
  using JetBrains.Annotations;

  internal sealed class TestcontainersConfigurationConverter
  {
    private const string UdpPortSuffix = "/udp";

    private const string TcpPortSuffix = "/tcp";

    private const string SctpPortSuffix = "/sctp";

    public TestcontainersConfigurationConverter(ITestcontainersConfiguration configuration)
    {
      this.Entrypoint = new ToCollection().Convert(configuration.Entrypoint)?.ToList();
      this.Command = new ToCollection().Convert(configuration.Command)?.ToList();
      this.Environments = new ToMappedList().Convert(configuration.Environments)?.ToList();
      this.Labels = new ToDictionary().Convert(configuration.Labels)?.ToDictionary(item => item.Key, item => item.Value);
      this.ExposedPorts = new ToExposedPorts().Convert(configuration.ExposedPorts)?.ToDictionary(item => item.Key, item => item.Value);
      this.PortBindings = new ToPortBindings().Convert(configuration.PortBindings)?.ToDictionary(item => item.Key, item => item.Value);
      this.Mounts = new ToMounts().Convert(configuration.Mounts)?.ToList();
      this.Networks = new ToNetworks().Convert(configuration.Networks)?.ToDictionary(item => item.Key, item => item.Value);
    }

    public IList<string> Entrypoint { get; }

    public IList<string> Command { get; }

    public IList<string> Environments { get; }

    public IDictionary<string, string> Labels { get; }

    public IDictionary<string, EmptyStruct> ExposedPorts { get; }

    public IDictionary<string, IList<PortBinding>> PortBindings { get; }

    public IList<Mount> Mounts { get; }

    public IDictionary<string, EndpointSettings> Networks { get; }

    private static string GetQualifiedPort(string containerPort)
    {
      return new[] { UdpPortSuffix, TcpPortSuffix, SctpPortSuffix }
        .Any(portSuffix => containerPort.EndsWith(portSuffix, StringComparison.OrdinalIgnoreCase))
        ? containerPort.ToLowerInvariant()
        : containerPort + TcpPortSuffix;
    }

    private sealed class ToCollection : CollectionConverter<string, string>
    {
      public ToCollection()
        : base(nameof(ToCollection))
      {
      }

      public override IEnumerable<string> Convert([CanBeNull] IEnumerable<string> source)
      {
        return source;
      }
    }

    private sealed class ToMounts : CollectionConverter<IMount, Mount>
    {
      public ToMounts()
        : base(nameof(ToMounts))
      {
      }

      public override IEnumerable<Mount> Convert([CanBeNull] IEnumerable<IMount> source)
      {
        return source?.Select(mount =>
        {
          var readOnly = AccessMode.ReadOnly.Equals(mount.AccessMode);
          if (mount is IBindMount bindMount)
          {
            return new Mount { Type = "bind", Source = bindMount.HostPath, Target = mount.ContainerPath, ReadOnly = readOnly };
          }

          if (mount is IVolumeMount volumeMount)
          {
            return new Mount { Type = "volume", Source = volumeMount.Volume.Name, Target = mount.ContainerPath, ReadOnly = readOnly };
          }

          throw new NotSupportedException("Unsupported mount type.");
        });
      }
    }

    private sealed class ToNetworks : CollectionConverter<IDockerNetwork, KeyValuePair<string, EndpointSettings>>
    {
      public ToNetworks()
        : base(nameof(ToNetworks))
      {
      }

      public override IEnumerable<KeyValuePair<string, EndpointSettings>> Convert([CanBeNull] IEnumerable<IDockerNetwork> source)
      {
        return source?.Select(network => new KeyValuePair<string, EndpointSettings>(network.Name, new EndpointSettings { NetworkID = network.Id }));
      }
    }

    private sealed class ToMappedList : DictionaryConverter<IEnumerable<string>>
    {
      public ToMappedList()
        : base(nameof(ToMappedList))
      {
      }

      public override IEnumerable<string> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(item => $"{item.Key}={item.Value}");
      }
    }

    private sealed class ToDictionary : DictionaryConverter<IEnumerable<KeyValuePair<string, string>>>
    {
      public ToDictionary()
        : base(nameof(ToDictionary))
      {
      }

      public override IEnumerable<KeyValuePair<string, string>> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        return source;
      }
    }

    private sealed class ToExposedPorts : DictionaryConverter<IEnumerable<KeyValuePair<string, EmptyStruct>>>
    {
      public ToExposedPorts()
        : base(nameof(ToExposedPorts))
      {
      }

      public override IEnumerable<KeyValuePair<string, EmptyStruct>> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(exposedPort => new KeyValuePair<string, EmptyStruct>(
          GetQualifiedPort(exposedPort.Key), default));
      }
    }

    private sealed class ToPortBindings : DictionaryConverter<IEnumerable<KeyValuePair<string, IList<PortBinding>>>>
    {
      public ToPortBindings()
        : base(nameof(ToPortBindings))
      {
      }

      public override IEnumerable<KeyValuePair<string, IList<PortBinding>>> Convert([CanBeNull] IEnumerable<KeyValuePair<string, string>> source)
      {
        return source?.Select(portBinding => new KeyValuePair<string, IList<PortBinding>>(
          GetQualifiedPort(portBinding.Key), new[] { new PortBinding { HostPort = "0".Equals(portBinding.Value, StringComparison.Ordinal) ? null : portBinding.Value } }));
      }
    }
  }
}
