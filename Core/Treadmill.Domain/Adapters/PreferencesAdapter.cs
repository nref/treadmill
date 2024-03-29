﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace Treadmill.Domain.Adapters
{
  public delegate void PreferenceChangedEvent(string name, object value);

  public interface IPreferencesProvider
  {
    string RemoteTreadmillServiceUrl { get; set; }
    string GpioClientRemoteUrl { get; set; }

    string LocalIp { get; set; }
    int LocalHttpPort { get; set; }
    string LocalUrl { get; }

    int LocalUdpPort { get; set; }
    int LocalUdpHealthPort { get; set; }
    int LocalUdpMetricsPort { get; set; }

    string MetricsIp { get; set; }
    string ListenUri { get; set; }
  }

  public interface IPreferencesAdapter : IPreferencesProvider
  {
    event PreferenceChangedEvent PreferenceChanged;
  }

  public abstract class PreferencesAdapter : IPreferencesAdapter
  {
    public PreferencesAdapter()
    {
      if (!Seeded)
      {
        Seed();
      }
    }

    private bool Seeded => Get<string>(nameof(LocalIp)) != default;

    private void Seed()
    {
      RemoteTreadmillServiceUrl = "http://192.168.1.164:8000";
      GpioClientRemoteUrl = "http://192.168.1.164:8001";

      // Sort by zero count most specific addresses first, i.e. 192.168.1.216 before 192.0.0.2.
      List<string> ips = Dns.GetHostAddresses(Dns.GetHostName()).Select(ip => $"{ip}").ToList();
      ips.Sort((ip1, ip2) => ip1.Count(c => c == '0') - ip2.Count(c => c == '0'));
      LocalIp = ips.First();

      LocalHttpPort = 8080;
      LocalUdpPort = 7889;
      LocalUdpHealthPort = 7890;
      LocalUdpMetricsPort = 7887;

      MetricsIp = "192.168.1.164";
      ListenUri = $"http://{LocalIp}:8002/";
    }

    public event PreferenceChangedEvent PreferenceChanged;

    public string RemoteTreadmillServiceUrl { get => Get<string>(); set => Set(value); }
    public string LocalIp
    {
      get => Get<string>();
      set
      {
        Set(value);
        NotifyPreferenceChanged(nameof(LocalUrl), value);
      }
    }

    public int LocalHttpPort
    {
      get => Get<int>();
      set
      {
        Set(value);
        NotifyPreferenceChanged(nameof(LocalUrl), value);
      }
    }

    public string LocalUrl { get => $"http://{LocalIp}:{LocalHttpPort}/callbacks/metrics/http/"; }

    public int LocalUdpPort { get => Get<int>(); set => Set(value); }
    public int LocalUdpHealthPort { get => Get<int>(); set => Set(value); }
    public int LocalUdpMetricsPort { get => Get<int>(); set => Set(value); }

    public string MetricsIp { get => Get<string>(); set => Set(value); }
    public string GpioClientRemoteUrl { get => Get<string>(); set => Set(value); }
    public string ListenUri { get => Get<string>(); set => Set(value); }

    protected abstract T Get<T>([CallerMemberName] string key = "");
    protected abstract void Set<T>(T value, [CallerMemberName] string key = "");

    protected void NotifyPreferenceChanged(string name, object value)
    {
      PreferenceChanged?.Invoke(name, value);
    }
  }
}
