using Caliburn.Micro;

namespace Treadmill.Ui.ViewModels
{

    public interface IMainViewModel
    {
    }

    public partial class MainViewModel : Conductor<object>.Collection.OneActive, IMainViewModel
    {
        public MainViewModel
        (
            IControlsViewModel controlsViewModel, 
            IWorkoutsViewModel workoutsViewModel,
            ILogViewModel logViewModel,
            ISettingsViewModel settingsViewModel
        )
        {
            foreach (var item in new object[] 
            { 
                controlsViewModel, 
                workoutsViewModel, 
                logViewModel, 
                settingsViewModel
            })
            {
                Items.Add(item);
            }

            ActivateItemAsync(Items[1], new System.Threading.CancellationToken());
        }
    }
}
