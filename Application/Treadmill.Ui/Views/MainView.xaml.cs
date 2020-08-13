using Treadmill.Ui.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Treadmill.Ui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : TabbedPage
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