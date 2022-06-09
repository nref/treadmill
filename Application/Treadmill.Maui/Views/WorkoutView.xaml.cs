using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

  [XamlCompilation(XamlCompilationOptions.Compile)]
  public partial class WorkoutView : ContentView
  {
      private readonly IWorkoutViewModel _viewModel;

      public WorkoutView(IWorkoutViewModel viewModel)
      {
          _viewModel = viewModel;
          BindingContext = _viewModel;
          InitializeComponent();
      }
  }
