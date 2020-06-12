using Precor956i.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Precor956i.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WorkoutsView : ContentView
    {
        private readonly IWorkoutViewModel _viewModel;
        private readonly ContentPage _workoutView;

        public WorkoutsView(IWorkoutViewModel viewModel)
        {
            _viewModel = viewModel;
            _workoutView = new ContentPage { Content = new WorkoutView(_viewModel) };

            BindingContext = _viewModel;

            InitializeComponent();
        }

        private void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Navigation.PushAsync(_workoutView);
        }
    }
}