using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace Treadmill.Domain.Adapters
{
    public delegate void PreferenceChangedEvent(string name, object value);

    public interface IPreferencesProvider
    {
        string RemoteUrl { get; set; }

        string LocalIp { get; set; }
        int LocalHttpPort { get; set; }
        string LocalUrl { get; }

        int LocalUdpPort { get; set; }
        int LocalUdpHealthPort { get; set; }
        int LocalUdpMetricsPort { get; set; }
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

        private bool Seeded => Get<string>(nameof(RemoteUrl)) != default;

        private void Seed()
        {
            RemoteUrl = "http://192.168.1.164:8000";
            LocalIp = Dns.GetHostAddresses(Dns.GetHostName()).First().ToString();
            LocalHttpPort = 8080;
            LocalUdpPort = 7889;
            LocalUdpHealthPort = 7890;
            LocalUdpMetricsPort = 7887;
        }

        public event PreferenceChangedEvent PreferenceChanged;

        public string RemoteUrl { get => Get<string>(); set => Set(value); }
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

        protected abstract T Get<T>([CallerMemberName] string key = "");
        protected abstract void Set<T>(T value, [CallerMemberName] string key = "");

        protected void NotifyPreferenceChanged(string name, object value)
        {
            PreferenceChanged?.Invoke(name, value);
        }
    }
}
