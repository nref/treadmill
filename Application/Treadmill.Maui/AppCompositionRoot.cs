using System.Reflection;
using Autofac;
using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;
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

      builder.RegisterType<AlertService>().As<IAlertService>();
      builder.RegisterType<MauiPreferencesAdapter>().As<IPreferencesAdapter>();

      builder.RegisterType<ControlsViewModel>().As<IControlsViewModel>();
      builder.RegisterType<LogViewModel>().As<ILogViewModel>();
      builder.RegisterType<MainViewModel>().As<IMainViewModel>();
      builder.RegisterType<SettingsViewModel>().As<ISettingsViewModel>();
      builder.RegisterType<WorkoutsViewModel>().As<IWorkoutsViewModel>().SingleInstance();
      builder.RegisterType<WorkoutViewModel>().As<IWorkoutViewModel>().SingleInstance();

      builder.RegisterType<ControlsView>();
      builder.RegisterType<LogView>();
      builder.RegisterType<MainView>();
      builder.RegisterType<SettingsView>();
      builder.RegisterType<WorkoutsView>();
      builder.RegisterType<WorkoutView>();
    }

    public override IEnumerable<Assembly> Assemblies => new[]
    {
      Assembly.GetExecutingAssembly(),
      typeof(Treadmill.Models.Log).Assembly,
      typeof(Treadmill.Domain.Services.IAlertService).Assembly,
    };
  }
}
