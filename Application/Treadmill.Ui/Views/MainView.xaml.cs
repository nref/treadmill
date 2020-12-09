using Treadmill.Ui.Styles;
using Treadmill.Ui.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Treadmill.Ui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView : TabbedPage
    {
        const int smallWightResolution = 768;
        const int smallHeightResolution = 1280;

        private IMainViewModel _viewModel;

        public MainView(IMainViewModel viewModel)
        {
            _viewModel = viewModel;
            BindingContext = _viewModel;

            InitializeComponent();
            //LoadStyles();
        }

        void LoadStyles()
        {
            if (IsASmallDevice())
            {
                styles.MergedDictionaries.Add(SmallDevicesStyle.SharedInstance);
            }
            else
            {
                styles.MergedDictionaries.Add(GeneralDevicesStyle.SharedInstance);
            }
        }

        public static bool IsASmallDevice()
        {
            // Get Metrics
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;

            // Width (in pixels)
            var width = mainDisplayInfo.Width;

            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            return width <= smallWightResolution && height <= smallHeightResolution;
        }
    }
}