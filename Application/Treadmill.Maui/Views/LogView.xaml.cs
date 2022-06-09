using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LogView : ContentView
{
  private readonly ILogViewModel _viewModel;

  public LogView(ILogViewModel viewModel)
  {
    _viewModel = viewModel;
    BindingContext = _viewModel;

    InitializeComponent();
  }
}
