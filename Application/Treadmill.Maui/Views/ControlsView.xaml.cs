using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ControlsView : ContentView
{
  private readonly IControlsViewModel _viewModel;

  public ControlsView(IControlsViewModel viewModel)
  {
    _viewModel = viewModel;
    BindingContext = _viewModel;

    InitializeComponent();
  }
}
