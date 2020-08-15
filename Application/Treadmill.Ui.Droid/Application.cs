using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using Ninject;
using Ninject.Extensions.Conventions;
using Treadmill.Ui.DomainServices;
using Treadmill.Ui.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using Treadmill.Domain.Adapters;
using Treadmill.Adapters.RemoteTreadmill;
using Treadmill.Domain.Services;

namespace Treadmill.Ui.Droid
{
    [Application(UsesCleartextTraffic = true)]
    public class Application : CaliburnApplication
    {
        private IKernel _container;

        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();

            Initialize();
        }

        protected override void Configure()
        {
            var settings = new NinjectSettings() { LoadExtensions = false };
            _container = new StandardKernel(settings);
            var config = new XamarinPreferencesAdapter();

            _container.Bind(x =>
            {
                x.From(SelectAssemblies())
                 .SelectAllClasses()
                 .BindDefaultInterface();
            });

            _container.Rebind<IWorkoutViewModel>()
                .To<WorkoutViewModel>()
                .InSingletonScope();

            _container.Rebind<IPreferencesAdapter>()
                .ToConstant(config);

            _container.Rebind<ILogService>()
                .To<LogService>()
                .InSingletonScope();

            _container.Rebind<IConnectionService>()
                .To<ConnectionService>()
                .InSingletonScope();

            _container.Rebind<IHttpService>().To<HttpService>()
                .WithConstructorArgument("url", config.LocalUrl);

            _container.Rebind<IRemoteTreadmillAdapter>()
                .To<RemoteTreadmillAdapter>()
                .InSingletonScope()
                .WithConstructorArgument("udpMetrics", new UdpService(config.LocalIp, config.LocalUdpPort))
                .WithConstructorArgument("health", new UdpService(config.LocalIp, config.LocalUdpHealthPort));

            _container.Rebind<IRemoteTreadmillClient>()
                .To<RemoteTreadmillClient>()
                .WithConstructorArgument("remoteUrl", config.RemoteUrl);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] 
            {
                Assembly.GetExecutingAssembly(), 
                typeof(MainViewModel).Assembly
            };
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.Get(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.Inject(instance);
        }
    }
}
