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
                MetricsIp = "192.168.1.164",
                MetricsPort = 7887,
                GpioClientRemoteUrl = "http://zerow2:8001" ,
                ListenUri = "http://localhost:8002/"
            };

            new SelfHost(new ApiCompositionRoot(config), new LogService())
                .Run(config.ListenUri);
        }
    }
}
