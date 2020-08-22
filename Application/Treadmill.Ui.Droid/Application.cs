using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Reflection;
using Treadmill.Infrastructure;
using Treadmill.Ui.DomainServices;

namespace Treadmill.Ui.Droid
{
    [Application(UsesCleartextTraffic = true)]
    public class Application : CaliburnApplication
    {
        private DroidCompositionRoot _container;

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
            var prefs = new XamarinPreferencesAdapter();
            var config = new DomainConfiguration(prefs);
            _container = new DroidCompositionRoot(config);
        }

        protected override IEnumerable<Assembly> SelectAssemblies() => DroidCompositionRoot.SelectAssemblies();
        protected override object GetInstance(Type service, string key) => _container.Get(service);
        protected override IEnumerable<object> GetAllInstances(Type service) => _container.GetAll(service);
        protected override void BuildUp(object instance) => _container.Inject(instance);
    }
}
