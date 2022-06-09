using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class WorkoutsView : ContentView
{
  private readonly IWorkoutsViewModel _viewModel;

  public WorkoutsView(IWorkoutsViewModel viewModel)
  {
    _viewModel = viewModel;
    BindingContext = _viewModel;
    InitializeComponent();
  }
}
