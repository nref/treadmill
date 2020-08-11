using Treadmill.Ui.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Treadmill.Ui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ControlsView : ContentView
    {
        private readonly IControlsViewModel _viewModel;

        public ControlsView(IControlsViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }
    }
}