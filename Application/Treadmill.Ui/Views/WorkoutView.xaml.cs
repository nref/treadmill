using Treadmill.Ui.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Treadmill.Ui.Views
{
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
}