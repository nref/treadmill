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
    private bool _active;
    private bool _paused;

    public event PropertyChangedEventHandler PropertyChanged;
    public event MetricChangedEvent SpeedChanged;
    public event MetricChangedEvent InclineChanged;

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

    public RemoteTreadmillService(IRemoteTreadmillAdapter treadmill)
    {
      _treadmill = treadmill;
      _treadmill.SpeedChanged += value => SpeedChanged?.Invoke(value);
      _treadmill.InclineChanged += value => InclineChanged?.Invoke(value);
    }

    public async Task<bool> Start()
    {
      Log.Add($"Beginning workout");
      Active = true;
      return await _treadmill.Start();
    }

    public async Task<bool> Pause()
    {
      Paused = true;

      Log.Add($"Pausing workout");
      return await _treadmill.Pause();
    }

    public async Task<bool> Resume()
    {
      Paused = false;

      Log.Add($"Resuming workout");
      return await _treadmill.Resume();
    }

    public async Task<bool> End()
    {
      Log.Add($"Ending workout");

      bool ok = await _treadmill.End();

      if (ok)
      {
        Paused = false;
        Active = false;
      }

      return ok;
    }

    public async Task<bool> GoToSpeed(double setpoint) => await _treadmill.GoToSpeed(setpoint);
    public async Task<bool> GoToIncline(double setpoint) => await _treadmill.GoToIncline(setpoint);

    private void NotifyPropertyChanged([CallerMemberName] string property = default)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
  }
}
