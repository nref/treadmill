using System.Threading.Tasks;

namespace Treadmill.Domain.Adapters
{
    public delegate void MetricChangedEvent(double value);

    public interface IRemoteTreadmillAdapter
    {
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
}
