using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Precor956i.ViewModels;

namespace Precor956i.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogView : ContentView
    {
        private readonly ILogViewModel _viewModel;

        public LogView(ILogViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
        }
    }
}