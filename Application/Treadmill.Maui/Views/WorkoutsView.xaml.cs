using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

public partial class WorkoutsView : ContentPage
{
  public WorkoutView WorkoutView { get; }

  public WorkoutsView(IWorkoutsViewModel vm, WorkoutView workoutView)
  {
    BindingContext = vm;
    WorkoutView = workoutView;
    InitializeComponent();
  }
}
