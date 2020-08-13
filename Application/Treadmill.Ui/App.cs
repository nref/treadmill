using Ninject;
using Treadmill.Ui.Views;
using Xamarin.Forms;

namespace Treadmill.Ui
{
    public class App : Application
    {
        private readonly IKernel _container;

        public App(IKernel container)
        {
            _container = container;

            MainPage = _container.Get<MainView>();
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
