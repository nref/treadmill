using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Precor956i.Clients;
using Precor956i.Dto;
using Precor956i.Infrastructure;
using Precor956i.Models;

namespace Precor956i.DomainServices
{
    public delegate void MetricChangedEvent(double value);

    public interface ITreadmillDomainService
    {
        event MetricChangedEvent SpeedChanged;
        event MetricChangedEvent InclineChanged;

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

    public class TreadmillDomainService : ITreadmillDomainService
    {
        public event MetricChangedEvent SpeedChanged;
        public event MetricChangedEvent InclineChanged;

        private readonly ILoggingService _logger;
        private readonly IConnectionService _connections;
        private readonly IDomainConfiguration _config;
        private readonly ITreadmillClient _client;
        private readonly HttpListener _httpListener = new HttpListener();

        private bool _connected = false;
        private DateTime _lastCallback = DateTime.UtcNow;
        private const int CallbackTimeoutSeconds = 15;

        private double _speed;
        private double _incline;

        public double Speed
        {
            get => _speed;
            set
            {
                if (Math.Abs(value - _speed) < Utility.ZERO)
                    return;
                _speed = value;
                SpeedChanged?.Invoke(value);
            }
        }

        public double Incline
        {
            get => _incline;
            set
            {
                if (Math.Abs(value - _incline) < Utility.ZERO)
                    return;
                _incline = value;
                InclineChanged?.Invoke(value);
            }
        }

        public TreadmillDomainService(ILoggingService logger, IConnectionService connections, IDomainConfiguration config, ITreadmillClient client)
        {
            _logger = logger;
            _connections = connections;
            _config = config;
            _client = client;

            Task.Run(() => ServeHttp());
            Task.Run(() => ServeUdp(new IPEndPoint(IPAddress.Parse(_config.LocalIp), _config.LocalUdpPort), HandleHealthCallback));
            Task.Run(() => ServeUdp(new IPEndPoint(IPAddress.Parse(_config.LocalIp), _config.LocalUdpHealthPort), ParseJson));
            Task.Run(() => ManageRegistration());
            Task.Run(() => Poll());
        }

        public async Task<bool> AddHttpMetricsCallback(string uri)
        {
            return await _client.AddHttpMetricsCallback(uri);
        }

        public async Task<bool> AddUdpMetricsCallback(string ip, int port)
        {
            return await _client.AddUdpMetricsCallback(ip, port);
        }

        public async Task<bool> AddUdpHealthCallback(string ip, int port)
        {
            return await _client.AddUdpHealthCallback(ip, port);
        }

        public async Task<bool> Start()
        {
            return await SafeExecAsync(async () => await _client.Start());
        }

        public async Task<bool> End()
        {
            return await SafeExecAsync(async () => await _client.End());
        }

        public async Task<bool> Pause()
        {
            return await SafeExecAsync(async () => await _client.Pause());
        }

        public async Task<bool> Resume()
        {
            return await SafeExecAsync(async () => await _client.Resume());
        }

        public async Task<bool> GoToSpeed(double setpoint)
        {
            return await _client.GoToSpeed(setpoint);
        }

        public async Task<bool> GoToIncline(double setpoint)
        {
            return await _client.GoToIncline(setpoint);
        }

        public async Task<double> GetSpeedFeedback()
        {
            return await _client.GetSpeedFeedback();
        }

        public async Task<double> GetInclineFeedback()
        {
            return await _client.GetInclineFeedback();
        }

        private void ServeUdp(IPEndPoint endpoint, Action<string> callback)
        {
            var client = new UdpClient(endpoint.Port);

            while (true)
            {
                var data = client.Receive(ref endpoint);
                _lastCallback = DateTime.UtcNow;
                var message = System.Text.Encoding.UTF8.GetString(data);
                callback(message);
            }
        }

        private void ServeHttp()
        {
            _httpListener.Prefixes.Add(_config.LocalUrl);
            _httpListener.Start();

            while (true)
            {
                var context = _httpListener.GetContext();
                _lastCallback = DateTime.UtcNow;
                var json = Read(context.Request);

                if (context.Request.Url.AbsoluteUri.EndsWith("callbacks/metrics/"))
                {
                    ParseJson(json);
                }

                Write(context.Response, HttpStatusCode.OK, "ok");
            }
        }

        private void HandleHealthCallback(string message)
        {

        }

        private void ParseJson(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<TreadmillMetric>(json);

                switch (obj.metric)
                {
                    case 1:
                        Speed = Convert.ToDouble(obj.value);
                        break;
                    case 2:
                        Incline = Convert.ToDouble(obj.value);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{json}: {e.Message}");
            }
        }

        private string Read(HttpListenerRequest request)
        {
            var reader = new StreamReader(request.InputStream);
            var ret = reader.ReadToEnd();
            reader.Close();
            return ret;
        }

        private void Write(HttpListenerResponse response, HttpStatusCode code, string responseString)
        {
            response.StatusCode = (int)code;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }

        private void HandleDisconnected()
        {
            _connected = false;
            _connections.NotifyConnectionChanged("Disconnected");
        }

        private async void HandleConnected()
        {
            await SafeExecAsync(async () =>
            {
                //bool ok = await _client.AddHttpMetricsCallback(_config.LocalUrl);
                bool ok = await AddUdpMetricsCallback(_config.LocalIp, _config.LocalUdpPort);
                if (ok)
                {
                    _connected = true;
                    _connections.NotifyConnectionChanged("Connected");
                }
            });
        }

        private async void ManageRegistration()
        {
            while (true)
            {
                if ((DateTime.UtcNow - _lastCallback).TotalSeconds > CallbackTimeoutSeconds)
                {
                    _lastCallback = DateTime.UtcNow;
                    HandleDisconnected();
                }

                if (!_connected)
                    await Register();

                Thread.Sleep(1000);
            }
        }

        private async Task Register()
        {
            await SafeExecAsync(async () =>
            {
                var ok = await AddUdpHealthCallback(_config.LocalIp, _config.LocalUdpHealthPort);

                if (!ok)
                {
                    HandleDisconnected();
                    return;
                }
                else
                {
                    HandleConnected();
                }
            });
        }

        private void Poll()
        {
            while (true)
            {
                PollOnce();
                Thread.Sleep(5 * 1000);
            }
        }

        private async void PollOnce()
        {
            Speed = await SafeExecAsync(async () => await GetSpeedFeedback());
            Incline = await SafeExecAsync(async () => await GetInclineFeedback());
        }

        private async Task SafeExecAsync(Func<Task> f)
        {
            try
            {
                await f();
            }
            catch (Exception e)
            {
                _connected = false;
                _logger.Log(e.Message);
            }
        }

        private async Task<T> SafeExecAsync<T>(Func<Task<T>> f)
        {
            try
            {
                return await f();
            }
            catch (Exception e)
            {
                _logger.Log(e.Message);
                return default;
            }
        }
    }
}
