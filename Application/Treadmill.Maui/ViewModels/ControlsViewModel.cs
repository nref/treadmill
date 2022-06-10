using System.Windows.Input;
using Treadmill.Models;
using Treadmill.Domain.Services;
using Treadmill.Maui.Views;

namespace Treadmill.Maui.ViewModels;

public interface IControlsViewModel
{
}

public class ControlsViewModel : ViewModel, IControlsViewModel
{
  private readonly IRemoteTreadmillService _service;
  private readonly IConnectionService _connections;

  private string _connectionStatus = "---";
  private string _speedEntry = "0.0";
  private string _inclineEntry = "0.0";
  private double _speed;
  private double _incline;

  public string ConnectionStatus
  {
    get => _connectionStatus;
    set
    {
      if (value == _connectionStatus)
        return;
      _connectionStatus = value;
      NotifyPropertyChanged();
    }
  }

  public string SpeedEntry
  {
    get => _speedEntry;
    set
    {
      if (value == _speedEntry)
        return;
      _speedEntry = value;
      NotifyPropertyChanged();
    }
  }

  public string InclineEntry
  {
    get => _inclineEntry;
    set
    {
      if (value == _inclineEntry)
        return;
      _inclineEntry = value;
      NotifyPropertyChanged();
    }
  }

  public double Speed
  {
    get => _speed;
    set
    {
      if (Math.Abs(value - _speed) < MathExtensions.ZERO)
        return;
      _speed = value;
      NotifyPropertyChanged();
    }
  }

  public double Incline
  {
    get => _incline;
    set
    {
      if (Math.Abs(value - _incline) < MathExtensions.ZERO)
        return;
      _incline = value;
      NotifyPropertyChanged();
    }
  }

  public ICommand HandleStart { private set; get; }
  public ICommand HandleEnd { private set; get; }
  public ICommand HandlePause { private set; get; }
  public ICommand HandleResume { private set; get; }
  public ICommand HandleSpeedUp { private set; get; }
  public ICommand HandleSpeedDown { private set; get; }
  public ICommand HandleInclineUp { private set; get; }
  public ICommand HandleInclineDown { private set; get; }
  public ICommand HandleGoToSpeed { private set; get; }
  public ICommand HandleGoToIncline { private set; get; }

  public ControlsViewModel
  (
    IConnectionService connections,
    IRemoteTreadmillService service
  )
  {
    _connections = connections;
    _service = service;

    _connections.ConnectionChanged += HandleConnectionChanged;
    service.SpeedChanged += HandleSpeedChanged;
    service.InclineChanged += HandleInclineChanged;

    HandleStart = new Command(async () => await _service.Start());
    HandleEnd = new Command(async () => await _service.End());
    HandlePause = new Command(async () => await _service.Pause());
    HandleResume = new Command(async () => await _service.Resume());
    HandleSpeedUp = new Command(async () => await _service.GoToSpeed(Speed + 0.1));
    HandleSpeedDown = new Command(async () => await _service.GoToSpeed(Speed - 0.1));
    HandleInclineUp = new Command(async () => await _service.GoToIncline(Incline + 0.5));
    HandleInclineDown = new Command(async () => await _service.GoToIncline(Incline - 0.5));
    HandleGoToSpeed = new Command(async () => await _service.GoToSpeed(Convert.ToDouble(SpeedEntry)));
    HandleGoToIncline = new Command(async () => await _service.GoToIncline(Convert.ToDouble(InclineEntry)));
  }

  private void HandleInclineChanged(double value)
  {
    Incline = value;
  }

  private void HandleSpeedChanged(double value)
  {
    Speed = value;
  }

  private void HandleConnectionChanged(string status)
  {
    ConnectionStatus = status;
  }
}
