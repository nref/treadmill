using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

public partial class LogView : ContentPage
{
  public LogView(ILogViewModel vm)
  {
    BindingContext = vm;
    InitializeComponent();
  }
}
