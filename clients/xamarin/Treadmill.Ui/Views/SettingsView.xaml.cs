using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Treadmill.Ui.ViewModels;

namespace Treadmill.Ui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsView : ContentView
    {
        private readonly ISettingsViewModel _viewModel;

        public SettingsView(ISettingsViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }
    }
}