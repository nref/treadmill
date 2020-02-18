using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace Precor956i.Infrastructure
{
    public interface IDomainConfiguration
    {
        string RemoteUrl { get; set; }

        string LocalIp { get; set; }
        int LocalHttpPort { get; set; }
        string LocalUrl { get; }

        int LocalUdpPort { get; set; }
        int LocalUdpHealthPort { get; set; }
    }

    public class DomainConfiguration : IDomainConfiguration
    {
        public string RemoteUrl { get => Get<string>(); set => Set(value); }
        public string LocalIp { get => Get<string>(); set => Set(value); }
        public string LocalUrl { get => $"http://{LocalIp}:{LocalHttpPort}/callbacks/metrics/http/"; }
        
        public int LocalHttpPort { get => Get<int>(); set => Set(value); }
        public int LocalUdpPort { get => Get<int>(); set => Set(value); }
        public int LocalUdpHealthPort { get => Get<int>(); set => Set(value); }

        public DomainConfiguration()
        {
            if (!Seeded())
                Seed();
        }

        private bool Seeded()
        {
            return Preferences.ContainsKey(nameof(RemoteUrl));
        }

        private void Seed()
        {
            RemoteUrl = "http://192.168.1.152:8000";
            LocalIp = "192.168.1.59";
            LocalHttpPort = 8080;
            LocalUdpPort = 7889;
            LocalUdpHealthPort = 7890;
        }

        private T Get<T>([CallerMemberName] string key = "")
        {
            if (!Preferences.ContainsKey(key))
                return default;
            string json = Preferences.Get(key, "");
            return JsonConvert.DeserializeObject<T>(json);
        }

        private void Set<T>(T value, [CallerMemberName] string key = "")
        {
            string json = JsonConvert.SerializeObject(value);
            Preferences.Set(key, json);
        }
    }
}
