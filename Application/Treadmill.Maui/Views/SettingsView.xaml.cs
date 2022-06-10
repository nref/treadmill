using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

public partial class SettingsView : ContentPage
{
  public SettingsView(ISettingsViewModel vm)
  {
    BindingContext = vm;
    InitializeComponent();
  }
}
