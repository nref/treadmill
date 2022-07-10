using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Treadmill.Domain.Adapters;
using Treadmill.Models;

namespace Treadmill.Domain.Services
{
  public interface IRemoteTreadmillService : INotifyPropertyChanged
  {
    bool Active { get; }
    bool Paused { get; }

    double Speed { get; }
    double Incline { get; }

    event MetricChangedEvent SpeedChanged;
    event MetricChangedEvent InclineChanged;
    event StateChangedEvent StateChanged;

    Task<bool> Start();
    Task<bool> End();
    Task<bool> Pause();
    Task<bool> Resume();

    Task<bool> GoToSpeed(double setpoint);
    Task<bool> GoToIncline(double setpoint);
  }

  public class RemoteTreadmillService : IRemoteTreadmillService
  {
    private readonly IRemoteTreadmillAdapter _treadmill;
    private readonly ILogService _log;
    private bool _active;
    private bool _paused;

    public event PropertyChangedEventHandler PropertyChanged;
    public event MetricChangedEvent SpeedChanged;
    public event MetricChangedEvent InclineChanged;
    public event StateChangedEvent StateChanged;

    public bool Active
    {
      get => _active;
      private set
      {
        if (_active == value)
        {
          return;
        }
        _active = value;
        NotifyPropertyChanged();
      }
    }
    public bool Paused
    {
      get => _paused;
      private set
      {
        if (_paused == value)
        {
          return;
        }
        _paused = value;
        NotifyPropertyChanged();
      }
    }

    public double Speed => _treadmill.Speed;
    public double Incline => _treadmill.Incline;
    public TreadmillState State => _treadmill.State;

    public RemoteTreadmillService(IRemoteTreadmillAdapter treadmill, ILogService log)
    {
      _treadmill = treadmill;
      _treadmill.SpeedChanged += value => SpeedChanged?.Invoke(value);
      _treadmill.InclineChanged += value => InclineChanged?.Invoke(value);
      _treadmill.StateChanged += value => StateChanged?.Invoke(value);
      _treadmill.StateChanged += HandleStateChanged;

      _log = log;
    }

    public async Task<bool> Start()
    {
      _log.Add($"Beginning workout");
      return await _treadmill.Start();
    }

    public async Task<bool> Pause()
    {
      _log.Add($"Pausing workout");
      return await _treadmill.Pause();
    }

    public async Task<bool> Resume()
    {
      _log.Add($"Resuming workout");
      return await _treadmill.Resume();
    }

    public async Task<bool> End()
    {
      _log.Add($"Ending workout");

      bool ok = await _treadmill.End();

      return ok;
    }

    public async Task<bool> GoToSpeed(double setpoint) => await _treadmill.GoToSpeed(setpoint);
    public async Task<bool> GoToIncline(double setpoint) => await _treadmill.GoToIncline(setpoint);

    private void HandleStateChanged(TreadmillState state)
    {
      _log.Add($"State is {state}");

      switch (state)
      {
        case TreadmillState.Ready:
          Active = false;
          Paused = false;
          break;
        case TreadmillState.Starting:
          Active = true;
          Paused = false;
          break;
        case TreadmillState.Started:
          Active = true;
          Paused = false;
          break;
        case TreadmillState.Paused:
          Active = true;
          Paused = true;
          break;
        case TreadmillState.Summary:
          Active = false;
          Paused = false;
          break;
      }
    }

    private void NotifyPropertyChanged([CallerMemberName] string property = default)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
