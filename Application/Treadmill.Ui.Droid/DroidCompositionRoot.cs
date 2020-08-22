﻿using Ninject.Extensions.Conventions;
using System.Collections.Generic;
using System.Reflection;
using Treadmill.Domain.Adapters;
using Treadmill.Infrastructure;
using Treadmill.Ui.DomainServices;
using Treadmill.Ui.ViewModels;

namespace Treadmill.Ui.Droid
{
    public class DroidCompositionRoot : CompositionRoot
    {
        public DroidCompositionRoot(DomainConfiguration config) : base(config)
        {
            Container.Bind(x =>
            {
                x.From(SelectAssemblies())
                 .SelectAllClasses()
                 .BindDefaultInterface();
            });

            Container.Bind<IPreferencesAdapter>().To<XamarinPreferencesAdapter>();
            Container.Rebind<IWorkoutViewModel>().To<WorkoutViewModel>().InSingletonScope();
        }

        public static IEnumerable<Assembly> SelectAssemblies() => new[]
            {
                Assembly.GetExecutingAssembly(),
                typeof(MainViewModel).Assembly,
            };
    }
}