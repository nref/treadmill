using Treadmill.Domain.Adapters;

namespace Treadmill.Infrastructure
{
    public class DomainConfiguration
    {
        public DomainConfiguration() { }
        public DomainConfiguration(IPreferencesAdapter config)
        {
            RemoteTreadmillServiceUrl = config.RemoteTreadmillServiceUrl;
            GpioClientRemoteUrl = config.GpioClientRemoteUrl;
            LocalIp = config.LocalIp;
            LocalHttpPort = config.LocalHttpPort;
            LocalUdpPort = config.LocalUdpPort;
            LocalUdpHealthPort = config.LocalUdpHealthPort;
            LocalUdpMetricsPort = config.LocalUdpMetricsPort;
            MetricsIp = config.MetricsIp;
            ListenUri = config.ListenUri;
        }

        public string RemoteTreadmillServiceUrl { get; set; }
        public string GpioClientRemoteUrl { get; set; }
        public string LocalIp { get; set; }
        public int LocalHttpPort { get; set; }
        public int LocalUdpPort { get; set; }
        public int LocalUdpHealthPort { get; set; }
        public int LocalUdpMetricsPort { get; set; }
        public string MetricsIp { get; set; }
        public string ListenUri { get; set; }
    }
}
