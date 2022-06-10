using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

public partial class MainView : Shell
{
  public MainView(IMainViewModel vm)
  {
    BindingContext = vm;
    InitializeComponent();
  }
}
