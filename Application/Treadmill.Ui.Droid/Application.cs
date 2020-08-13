using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using Ninject;
using Ninject.Extensions.Conventions;
using Treadmill.Ui.Clients;
using Treadmill.Ui.DomainServices;
using Treadmill.Ui.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            var config = new PreferencesService();

            _container.Bind(x =>
            {
                x.From(SelectAssemblies())
                 .SelectAllClasses()
                 .BindDefaultInterface();
            });

            _container.Rebind<IWorkoutViewModel>()
                .To<WorkoutViewModel>()
                .InSingletonScope();

            _container.Rebind<ILoggingService>()
                .To<LoggingService>()
                .InSingletonScope();

            _container.Rebind<IConnectionService>()
                .To<ConnectionService>()
                .InSingletonScope();

            _container.Rebind<ITreadmillService>()
                .To<TreadmillService>()
                .InSingletonScope();

            _container.Rebind<IPreferencesService>()
                .ToConstant(config);

            _container.Rebind<ITreadmillClient>()
                .To<TreadmillClient>()
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
