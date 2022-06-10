using Treadmill.Domain.Services;
using Treadmill.Maui.Models;
using Treadmill.Maui.Shared;

namespace Treadmill.Maui.ViewModels;

public interface ILogViewModel
{
}

public class LogViewModel : ViewModel, ILogViewModel
{
  private FullyObservableCollection<LogEntry> _log = new();

  public FullyObservableCollection<LogEntry> Log
  {
    get => _log;
    set
    {
      if (value == _log)
        return;
      _log = value;
      NotifyPropertyChanged();
    }
  }

  public LogViewModel()
  {
    Treadmill.Models.Log.Added += HandleAdded;

    Treadmill.Models.Log.Add("Logger ready");
  }

  private void HandleAdded(string message)
  {
    _ui.Post(state =>
    {
      Log.Insert(0, new LogEntry { Message = message });
    }, null);
  }
}
