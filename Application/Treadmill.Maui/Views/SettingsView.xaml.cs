using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SettingsView : ContentView
{
  private readonly ISettingsViewModel _viewModel;

  public SettingsView(ISettingsViewModel viewModel)
  {
    _viewModel = viewModel;
    BindingContext = _viewModel;

    InitializeComponent();
  }
}
