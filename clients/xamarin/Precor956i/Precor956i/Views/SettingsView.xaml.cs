using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Precor956i.ViewModels;

namespace Precor956i.Views
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