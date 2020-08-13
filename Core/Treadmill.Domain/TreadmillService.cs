using System.Threading.Tasks;
using Treadmill.Domain.Adapters;

namespace Treadmill.Domain
{
    public interface ITreadmillService
    {
        Task SetSpeed(double speed);
    }

    public class TreadmillService : ITreadmillService
    {
        private readonly ITreadmillAdapter _adapter;

        public TreadmillService(ITreadmillAdapter adapter)
        {
            _adapter = adapter;
        }

        public async Task SetSpeed(double speed)
        {
            await _adapter.IncrementSpeed();
        }
    }
}
