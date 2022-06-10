using Treadmill.Maui.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Treadmill.Maui;

public partial class App : Application
{
  public App(MainView page)
  {
    MainPage = page;
    InitializeComponent();
  }
}