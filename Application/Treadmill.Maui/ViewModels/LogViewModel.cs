using System.Collections.ObjectModel;
using Treadmill.Domain.Services;
using Treadmill.Maui.Models;

namespace Treadmill.Maui.ViewModels;

public interface ILogViewModel
{
}

public class LogViewModel : BindableObject, ILogViewModel
{
  private ObservableCollection<LogEntry> _log = new();

  public ObservableCollection<LogEntry> Log
  {
    get => _log;
    set
    {
      if (value == _log)
        return;
      _log = value;
      OnPropertyChanged();
    }
  }
  public string DisplayName { get; set; } = "Log";

  private readonly ILogService _logger;

  public LogViewModel(ILogService logger)
  {
    _logger = logger;
    _logger.EventLogged += HandleEventLogged;
    _logger = logger;

    HandleEventLogged("Logger ready");
  }

  private void HandleEventLogged(string message)
  {
    Log.Insert(0, new LogEntry { Message = message });
  }
}
