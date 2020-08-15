using Ninject;
using System.Collections.Generic;
using System.Linq;
using Treadmill.Adapters.Gpio;
using Treadmill.Domain;
using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;

namespace Treadmill.Infrastructure
{
    public interface ICompositionRoot
    {
        List<T> GetAll<T>();
    }

    public class CompositionRoot : ICompositionRoot
    {
        protected IKernel Container { get; private set; }

        public CompositionRoot(DomainConfiguration config)
        {
            Container = new StandardKernel(new NinjectSettings { LoadExtensions = false });

            Container.Bind<ILoggingService>().To<LoggingService>();

            Container.Bind<ITreadmillService>().To<TreadmillService>()
                .WithConstructorArgument("metrics", new UdpService(config.MetricsIp, config.MetricsPort));

            Container.Bind<ITreadmillAdapter>().To<TreadmillAdapter>();
            Container.Bind<ITreadmillClient>().To<GpioClient>().WithConstructorArgument("remoteUrl", config.GpioClientRemoteUrl);
        }

        public List<T> GetAll<T>() => Container.GetAll<T>().ToList();
    }
}
