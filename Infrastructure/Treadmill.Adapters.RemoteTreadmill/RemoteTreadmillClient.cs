using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Treadmill.Domain.Adapters
{
    public interface IRemoteTreadmillClient
    {
        /// <summary>
        /// Subscribe to metrics changes via HTTP.
        /// </summary>
        /// <param name="ipAndPort">Uri e.g. http://127.0.0.1/path/to/endpoint</param>
        Task<bool> AddHttpMetricsCallback(string uri);

        /// <summary>
        /// Subscribe to metrics changes via UDP.
        /// Much faster than HTTP but unreliable
        /// Must be a different port than that set via AddUdpHealthCallback
        /// </summary>
        Task<bool> AddUdpMetricsCallback(string ip, int port);

        /// <summary>
        /// Subscribe to regular health pings from the server via UDP.
        /// Must be a different port than that set via AddUdpMetricsCallback
        /// </summary>
        Task<bool> AddUdpHealthCallback(string ip, int port);

        Task<bool> Start();
        Task<bool> End();
        Task<bool> Pause();
        Task<bool> Resume();

        Task<bool> GoToSpeed(double setpoint);
        Task<bool> GoToIncline(double setpoint);

        Task<double> GetSpeedFeedback();
        Task<double> GetInclineFeedback();
    }

    public class RemoteTreadmillClient : IRemoteTreadmillClient
    {
        private readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        private readonly string _remoteUrl;

        public RemoteTreadmillClient(string remoteUrl)
        {
            _remoteUrl = remoteUrl;
        }

        public async Task<bool> AddHttpMetricsCallback(string uri)
        {
            return (await PostAsync("callbacks/metrics/http", uri)).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> AddUdpMetricsCallback(string ip, int port)
        {
            return (await PostAsync("callbacks/metrics/udp", $"{ip}:{port}")).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> AddUdpHealthCallback(string ip, int port)
        {
            return (await PostAsync("callbacks/health", $"{ip}:{port}")).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> Start()
        {
            return (await PostAsync("start")).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> End()
        {
            return (await PostAsync("end")).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> Pause()
        {
            return (await PostAsync("pause")).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> Resume()
        {
            return (await PostAsync("resume")).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> GoToSpeed(double setpoint)
        {
            return (await PostAsync("speed/setpoint", setpoint)).StatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> GoToIncline(double setpoint)
        {
            return (await PostAsync("incline/setpoint", setpoint)).StatusCode == HttpStatusCode.OK;
        }

        public async Task<double> GetInclineFeedback()
        {
            var speedString = await GetAsync($"incline/feedback");
            return Convert.ToDouble(speedString);
        }

        public async Task<double> GetSpeedFeedback()
        {
            var inclineString = await GetAsync($"speed/feedback");
            return Convert.ToDouble(inclineString);
        }

        private async Task<string> GetAsync(string route)
        {
            return await _client
                .GetStringAsync($"{_remoteUrl}/{route}")
                .ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> PostAsync(string route, object value = null)
        {
            var response = await _client
                .PostAsync($"{_remoteUrl}/{route}",
                    new StringContent(value != null ? value.ToString() : string.Empty))
                .ConfigureAwait(false);

            return response;
        }
    }
}
