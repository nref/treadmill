using Caliburn.Micro.Xamarin.Forms;
using Ninject;
using Precor956i.Views;
using Xamarin.Forms;

namespace Precor956i
{
    public class App : FormsApplication
    {
        private readonly IKernel _container;

        public App(IKernel container)
        {
            Initialize();
            _container = container;

            DisplayRootView<MainView>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        protected override void PrepareViewFirst(NavigationPage navigationPage)
        {
            _container.Bind<INavigationService>().ToConstant(new NavigationPageAdapter(navigationPage));
        }
    }
}
