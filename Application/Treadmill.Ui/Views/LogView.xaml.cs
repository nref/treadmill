using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Treadmill.Ui.ViewModels;

namespace Treadmill.Ui.Views
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