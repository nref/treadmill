using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class MainView : TabbedPage
{
  private IMainViewModel _viewModel;

  public MainView(IMainViewModel viewModel)
  {
    _viewModel = viewModel;
    BindingContext = _viewModel;

    InitializeComponent();
  }
}
