using Autofac;
using Autofac.Extensions.DependencyInjection;
using Maui.Plugins.PageResolver;
using Treadmill.Infrastructure;
using Treadmill.Maui.Adapters;

namespace Treadmill.Maui;

public static class MauiProgram
{
  public static MauiApp CreateMauiApp()
  {
    MauiAppBuilder builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .UsePageResolver()
      .ConfigureFonts(fonts =>
      {
        fonts.AddFont("OpenSansRegular.ttf", "OpenSansRegular");
      });

    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
    {
      var prefs = new MauiPreferencesAdapter();
      var config = new DomainConfiguration(prefs);
      var root = new AppCompositionRoot(config);

      root.Configure(builder);
    }));
    return builder.Build();
  }
}
