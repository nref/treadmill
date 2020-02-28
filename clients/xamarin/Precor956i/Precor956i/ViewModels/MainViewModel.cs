using Caliburn.Micro;

namespace Precor956i.ViewModels
{

    public interface IMainViewModel
    {
    }

    public partial class MainViewModel : Conductor<object>.Collection.OneActive, IMainViewModel
    {
        public MainViewModel
        (
            IControlsViewModel controlsViewModel, 
            IWorkoutViewModel workoutViewModel,
            ILogViewModel logViewModel,
            ISettingsViewModel settingsViewModel
        )
        {
            foreach (var item in new object[] 
            { 
                controlsViewModel, 
                workoutViewModel, 
                logViewModel, 
                settingsViewModel
            })
            {
                Items.Add(item);
            }

            ActivateItemAsync(Items[0], new System.Threading.CancellationToken());
        }
    }
}
