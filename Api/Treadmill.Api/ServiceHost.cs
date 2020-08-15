using System;
using System.Threading.Tasks;
using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;
using Treadmill.Hosting;
using Treadmill.Infrastructure;

namespace Treadmill.Api
{
    public interface IServiceHost 
    {
        void Run();
    }

    public class ServiceHost
    {
        private readonly IPreferencesAdapter _config;
        private readonly ILogService _logger;

        public ServiceHost(IPreferencesAdapter config, ILogService logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Run()
        {
            Task.Run(() =>
            {
                var dc = new DomainConfiguration
                {
                    MetricsIp = "192.168.1.164",
                    MetricsPort = 7887,
                    GpioClientRemoteUrl = "http://192.168.1.164:8001",
                    ListenUri = $"http://{_config.LocalIp}:8002/"
                };

                try
                {
                    new SelfHost(new ApiCompositionRoot(dc), _logger)
                        .Run(dc.ListenUri);
                }
                catch (Exception e)
                {
                    _logger.Add(e.Message);
                }
            });
        }
    }
}
