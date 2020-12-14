using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Treadmill.Adapters.Gpio
{
    public interface ITreadmillClient
    {
        Task<bool> Heartbeat();
        Task<bool> EnableHeartbeat();
        Task<bool> DisableHeartbeat();

        Task<bool> SetGpio(int gpio, bool on);
        Task<bool> GetGpio(int gpio);
        Task<Dictionary<int, bool>> GetGpios();
    }

    public class GpioClient : ITreadmillClient
    {
        private readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        private readonly string _remoteUrl;

        public GpioClient(string remoteUrl)
        {
            _remoteUrl = remoteUrl;
        }

        public async Task<bool> Heartbeat()
            => (await PostAsync($"heartbeat")).StatusCode == HttpStatusCode.OK;
        
        public async Task<bool> EnableHeartbeat()
            => (await PostAsync($"heartbeat/enable")).StatusCode == HttpStatusCode.OK;
        
        public async Task<bool> DisableHeartbeat()
            => (await PostAsync($"heartbeat/disable")).StatusCode == HttpStatusCode.OK;

        public async Task<bool> SetGpio(int gpio, bool on) 
            => (await PostAsync($"pin/{gpio}/{(on ? "on" : "off")}"))
                .StatusCode == HttpStatusCode.OK;

        public async Task<bool> GetGpio(int gpio)
        {
            var on = await GetAsync($"pin/{gpio}");
            return on == "1";
        }

        public async Task<Dictionary<int, bool>> GetGpios()
        {
            var json = await GetAsync($"pins");
            return JsonConvert.DeserializeObject<Dictionary<int, bool>>(json);
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
