using System.Reflection;
using Autofac;
using Treadmill.Domain.Adapters;
using Treadmill.Infrastructure;
using Treadmill.Maui.Adapters;
using Treadmill.Maui.ViewModels;
using Treadmill.Maui.Views;

namespace Treadmill.Maui
{
  public class AppCompositionRoot : CompositionRoot
  {
    public AppCompositionRoot(DomainConfiguration config) : base(config)
    {
    }

    public override void Configure(ContainerBuilder builder)
    {
      base.Configure(builder);

      builder.RegisterType<MauiPreferencesAdapter>().As<IPreferencesAdapter>();
      builder.RegisterType<WorkoutViewModel>().As<IWorkoutViewModel>().SingleInstance();
      builder.RegisterType<WorkoutsViewModel>().As<IWorkoutsViewModel>().SingleInstance();

      builder.RegisterType<MainView>();
      builder.RegisterType<ControlsView>();
      builder.RegisterType<SettingsView>();
      builder.RegisterType<LogView>();
      builder.RegisterType<WorkoutsView>();
      builder.RegisterType<WorkoutView>();
    }

    public override IEnumerable<Assembly> Assemblies => new[]
    {
      Assembly.GetExecutingAssembly(),
      typeof(MainViewModel).Assembly,
    };
  }
}
