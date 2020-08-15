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

        public TreadmillService(ITreadmillAdapter adapter, IUdpService metrics)
        {
            _adapter = adapter;
            _metrics = metrics;
            Task.Run(() => metrics.Serve(HandleMetricsChanged));
        }

        private void HandleMetricsChanged(string message)
        {
            System.Console.WriteLine($"HandleMetricsChanged({message.Replace("\n", "")})");
        }

        public async Task SetSpeed(double speed)
        {
            await _adapter.IncrementSpeed();
        }
    }
}
