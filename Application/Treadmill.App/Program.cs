using System.Linq;
using System.Net;
using Treadmill.Api;
using Treadmill.Domain.Services;
using Treadmill.Hosting;
using Treadmill.Infrastructure;

namespace Treadmill.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new DomainConfiguration 
            { 
                RemoteTreadmillServiceUrl = "http://192.168.1.164:8000",
                MetricsIp = "192.168.1.164",
                LocalIp = Dns.GetHostAddresses(Dns.GetHostName()).First().ToString(),
                LocalHttpPort = 8080,
                LocalUdpHealthPort = 7890,
                LocalUdpMetricsPort = 7887,
                GpioClientRemoteUrl = "http://zerow2:8001" ,
                ListenUri = "http://localhost:8002/"
            };

            new SelfHost(new ApiCompositionRoot(config), new LogService())
                .Run(config.ListenUri);
        }
    }
}
