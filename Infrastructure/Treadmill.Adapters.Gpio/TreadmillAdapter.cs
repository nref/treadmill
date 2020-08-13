using System.Threading;
using System.Threading.Tasks;
using Treadmill.Domain.Adapters;

namespace Treadmill.Adapters.Gpio
{
    public class TreadmillAdapter : ITreadmillAdapter
    {
        private readonly ITreadmillClient _client;

        public TreadmillAdapter(ITreadmillClient client)
        {
            _client = client;
            Task.Run(() => ServiceHeartbeat());
        }

        public async Task Start()
        {
            await PulseGpio_(0);
        }

        public async Task Stop()
        {
            await PulseGpio_(0);
        }

        public async Task Pause()
        {
            await PulseGpio_(0);
        }

        public async Task Resme()
        {
            await PulseGpio_(0);
        }

        public async Task DecrementIncline()
        {
            await PulseGpio_(0);
        }

        public async Task DecrementSpeed()
        {
            await PulseGpio_(0);
        }

        public async Task IncrementIncline()
        {
            await PulseGpio_(0);
        }

        public async Task IncrementSpeed()
        {
            await PulseGpio_(0);
        }

        private async Task PulseGpio_(int pin)
        {
            await _client.SetGpio(pin, true).ConfigureAwait(false);
            await Task.Delay(100).ConfigureAwait(false);
            await _client.SetGpio(pin, false).ConfigureAwait(false);
        }

        private async Task ServiceHeartbeat(CancellationToken ct = default)
        {
            await _client.EnableHeartbeat();

            while (!ct.IsCancellationRequested)
            {
                await _client.Heartbeat().ConfigureAwait(false);
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
    }
}
