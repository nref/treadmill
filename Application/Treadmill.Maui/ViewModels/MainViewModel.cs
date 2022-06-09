namespace Treadmill.Maui.ViewModels;

public interface IMainViewModel
{
}

public partial class MainViewModel : IMainViewModel
{
  public BindableObject ActiveItem { get; set; }
  public List<BindableObject> Items { get; } = new();

  public IControlsViewModel ControlsViewModel { get; }
  public IWorkoutsViewModel WorkoutsViewModel { get; }
  public ILogViewModel LogViewModel { get; }
  public ISettingsViewModel SettingsViewModel { get; }

  public MainViewModel
  (
      IControlsViewModel controlsViewModel,
      IWorkoutsViewModel workoutsViewModel,
      ILogViewModel logViewModel,
      ISettingsViewModel settingsViewModel
  )
  {
    ControlsViewModel = controlsViewModel;
    WorkoutsViewModel = workoutsViewModel;
    LogViewModel = logViewModel;
    SettingsViewModel = settingsViewModel;

    ControlsView = viewModels.GetViewFor<IControlsViewModel>();
    WorkoutsView = viewModels.GetViewFor<IWorkoutsViewModel>();
    LogView = viewModels.GetViewFor<ILogViewModel>();
    SettingsView = viewModels.GetViewFor<ISettingsViewModel>();

    Items.Add(ControlsView);
    Items.Add(WorkoutsView);
    Items.Add(LogView);
    Items.Add(SettingsView);

    ActiveItem = ControlsView;
  }
}
