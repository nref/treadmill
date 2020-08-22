using Ninject;
using Treadmill.Ui.Views;
using Xamarin.Forms;

namespace Treadmill.Ui
{
    public class App : Application
    {
        public App(IKernel container)
        {
            MainPage = container.Get<MainView>();
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
