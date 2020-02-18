using Ninject;
using Precor956i.Views;
using Xamarin.Forms;

namespace Precor956i
{
    public class App : Application
    {
        private readonly IKernel _container;

        public App(IKernel container)
        {
            _container = container;

            MainPage = new ContentPage { Content = _container.Get<MainView>() };
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
    }
}
