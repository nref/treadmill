using Lamar;
using System.Collections.Generic;
using System.Linq;
using Treadmill.Adapters.Gpio;
using Treadmill.Domain.Adapters;

namespace Treadmill.Infrastructure
{
    public interface ICompositionRoot
    {
        List<T> GetAll<T>();
    }

    public class CompositionRoot : ICompositionRoot
    {
        protected Container Container { get; private set; }

        private readonly DomainConfiguration _config;

        public CompositionRoot(DomainConfiguration config)
        {
            Container = new Container(registry =>
            {
                registry.Scan(scanner =>
                {
                    scanner.AssembliesFromApplicationBaseDirectory();
                    scanner.WithDefaultConventions();
                });

                registry.For<ITreadmillAdapter>().Use<TreadmillAdapter>();
                registry.For<ITreadmillClient>().Use<GpioClient>()
                    .Ctor<string>("remoteUrl").Is(config.GpioClientRemoteUrl);
            });
            _config = config;
        }

        public List<T> GetAll<T>() => Container.GetAllInstances<T>().ToList();
    }
}
