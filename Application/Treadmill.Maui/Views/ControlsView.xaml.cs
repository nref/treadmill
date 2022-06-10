using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

public partial class ControlsView : ContentPage
{
  public WorkoutView WorkoutView { get; }

  public ControlsView(IControlsViewModel vm, WorkoutView workoutView)
  {
    BindingContext = vm;
    WorkoutView = workoutView;
    InitializeComponent();
  }
}
