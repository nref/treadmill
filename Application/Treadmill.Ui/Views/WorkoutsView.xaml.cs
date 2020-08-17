using Treadmill.Ui.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Treadmill.Ui.Views
{
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
}