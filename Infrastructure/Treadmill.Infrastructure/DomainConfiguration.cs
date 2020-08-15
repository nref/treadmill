namespace Treadmill.Infrastructure
{
    public class DomainConfiguration
    {   
        public string MetricsIp { get; set; }
        public int MetricsPort { get; set; }
        public string GpioClientRemoteUrl { get; set; }
        public string ListenUri { get; set; }
    }
}
