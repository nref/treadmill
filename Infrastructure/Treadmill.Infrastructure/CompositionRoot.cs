using Ninject;
using System.Collections.Generic;
using System.Linq;
using Treadmill.Adapters.Gpio;
using Treadmill.Domain;
using Treadmill.Domain.Adapters;

namespace Treadmill.Infrastructure
{
    public interface ICompositionRoot
    {
        List<T> GetAll<T>();
    }

    public class CompositionRoot : ICompositionRoot
    {
        protected IKernel Container { get; private set; }

        private readonly DomainConfiguration _config;

        public CompositionRoot(DomainConfiguration config)
        {
            _config = config;
            Container = new StandardKernel(new NinjectSettings { LoadExtensions = false });

            //Container.Bind(x => x
            //    .FromAssembliesMatching("Treadmill.*")
            //    .SelectAllClasses()
            //    .BindDefaultInterface()
            //);

            Container.Bind<ITreadmillService>().To<TreadmillService>();
            Container.Bind<ITreadmillAdapter>().To<TreadmillAdapter>();
            Container.Bind<ITreadmillClient>().To<GpioClient>().WithConstructorArgument("remoteUrl", config.GpioClientRemoteUrl);
        }

        public List<T> GetAll<T>() => Container.GetAll<T>().ToList();
    }
}
