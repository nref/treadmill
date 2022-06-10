using Autofac;
using Autofac.Extensions.DependencyInjection;
using Treadmill.Infrastructure;
using Treadmill.Maui;
using Treadmill.Maui.Adapters;

namespace Treadmill.Maui
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {
      MauiAppBuilder builder = MauiApp.CreateBuilder();
      builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        })
        .ConfigureContainer(new AutofacServiceProviderFactory(builder =>
        {
          var prefs = new MauiPreferencesAdapter();
          var config = new DomainConfiguration(prefs);
          var root = new AppCompositionRoot(config);

          root.Configure(builder);
        }), builder =>
        {

        });

      return builder.Build();
    }

  }
}