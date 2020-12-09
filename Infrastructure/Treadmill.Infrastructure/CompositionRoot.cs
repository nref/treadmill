using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using Treadmill.Adapters.Gpio;
using Treadmill.Adapters.RemoteTreadmill;
using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;

namespace Treadmill.Infrastructure
{
    public interface ICompositionRoot
    {
        object Get(Type t);
        List<object> GetAll(Type t);
        T Get<T>();
        List<T> GetAll<T>();
        void Inject(object instance);
    }

    public class CompositionRoot : ICompositionRoot
    {
        protected IKernel Container { get; private set; }

        public CompositionRoot(DomainConfiguration config)
        {
            Container = new StandardKernel(new NinjectSettings { LoadExtensions = false });

            Container.Bind<IRemoteTreadmillAdapter>()
                .To<RemoteTreadmillAdapter>()
                .InSingletonScope()
                .WithConstructorArgument("udpMetrics", new UdpService(config.LocalIp, config.LocalUdpPort))
                .WithConstructorArgument("health", new UdpService(config.LocalIp, config.LocalUdpHealthPort));

            Container.Bind<IRemoteTreadmillClient>()
                .To<RemoteTreadmillClient>()
                .WithConstructorArgument("remoteUrl", config.RemoteTreadmillServiceUrl);

            Container.Bind<ITreadmillService>().To<TreadmillService>()
                .WithConstructorArgument("metrics", new UdpService(config.MetricsIp, config.LocalUdpMetricsPort));
            
                Container.Bind<ITreadmillAdapter>().To<TreadmillAdapter>();

            Container.Bind<ITreadmillClient>().To<GpioClient>()
                .WithConstructorArgument("remoteUrl", config.GpioClientRemoteUrl);

            Container.Bind<IHttpService>().To<HttpService>()
                .WithConstructorArgument("url", config.LocalUrl);

            Container.Bind<IRemoteTreadmillService>().To<RemoteTreadmillService>().InSingletonScope();
            Container.Bind<IConnectionService>().To<ConnectionService>().InSingletonScope();
            Container.Bind<ILogService>().To<LogService>().InSingletonScope();
        }

        public object Get(Type t) => Container.Get(t);
        public List<object> GetAll(Type t) => Container.GetAll(t).ToList();
        public T Get<T>() => Container.Get<T>();
        public List<T> GetAll<T>() => Container.GetAll<T>().ToList();

        public void Inject(object instance) => Container.Inject(instance);
    }
}
