using Autofac;
using Treadmill.Infrastructure;
using Treadmill.Maui.Adapters;
using Treadmill.Maui.Views;

namespace Treadmill.Maui
{
  public class AppCompositionRootFixture
  {
    /// <summary>
    /// Verify that the object graph can be constructed
    /// </summary>
    public void BuildsObjectGraph()
    {
      var prefs = new MauiPreferencesAdapter();
      var config = new DomainConfiguration(prefs);
      var root = new AppCompositionRoot(config);
      var builder = new ContainerBuilder();
      root.Configure(builder);
      Autofac.IContainer container = builder.Build();
      var app = container.Resolve<MainView>();
    }
  }
}