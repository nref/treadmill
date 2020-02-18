using Precor956i.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Precor956i.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : ContentView
    {
        private IMainViewModel _viewModel;

        public MainView(IMainViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }
    }
}