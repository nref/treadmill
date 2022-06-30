using System.Threading.Tasks;
using Treadmill.Models;

namespace Treadmill.Domain.Adapters
{
  public delegate void MetricChangedEvent(double value);
  public delegate void StateChangedEvent(TreadmillState state);

  public interface IRemoteTreadmillAdapter
  {
    double Speed { get; }
    double Incline { get; }
    TreadmillState State { get; }

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
}
