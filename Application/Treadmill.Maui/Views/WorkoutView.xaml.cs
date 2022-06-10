using Treadmill.Maui.ViewModels;

namespace Treadmill.Maui.Views;

public partial class WorkoutView : ContentView
{
  public WorkoutView(IWorkoutViewModel vm)
  {
    BindingContext = vm;
    InitializeComponent();
  }
}
