using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using Ninject;
using Ninject.Extensions.Conventions;
using Precor956i.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Precor956i.Droid
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

            _container.Bind(x =>
            {
                x.From(SelectAssemblies())
                 .SelectAllClasses()
                 .BindDefaultInterface();
            });

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
