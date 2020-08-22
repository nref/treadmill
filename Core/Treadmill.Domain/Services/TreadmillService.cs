using System.ComponentModel;
using System.Threading.Tasks;
using Treadmill.Domain.Adapters;

namespace Treadmill.Domain.Services
{
    public interface ITreadmillService
    {
        Task SetSpeed(double speed);
    }

    public class TreadmillService : ITreadmillService
    {
        private readonly ITreadmillAdapter _adapter;
        private readonly IUdpService _metrics;
        private readonly ILogService _log;

        public TreadmillService(ITreadmillAdapter adapter, IUdpService metrics, ILogService log)
        {
            _adapter = adapter;
            _metrics = metrics;
            _log = log;
            Task.Run(() => metrics.Serve(HandleMetricsChanged));
        }

        private void HandleMetricsChanged(string message)
        {
            _log.Add($"HandleMetricsChanged({message.Replace("\n", "")})");
        }

        public async Task SetSpeed(double speed)
        {
            await _adapter.IncrementSpeed();
        }
    }
}
