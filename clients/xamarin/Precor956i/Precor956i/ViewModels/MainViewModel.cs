using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Precor956i.ViewModels
{
    public interface IMainViewModel
    {

    }

    public class MainViewModel : BindableObject, IMainViewModel
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly UdpClient _udpClient = new UdpClient(7889);

        private readonly string _host = "http://192.168.1.152:8000";
        private readonly string _httpCallback = "http://192.168.1.151:8080/callbacks/metrics/http/";
        private readonly string _udpCallback = "192.168.1.151:7889";
        private readonly string _udpHealthCallback = "192.168.1.151:7890";

        private bool _connected = false;
        private DateTime _lastCallback = DateTime.UtcNow;
        private const int CallbackTimeoutSeconds = 15;

        private string _generalStatus = "---";
        private string _connectionStatus = "---";
        private string _speedEntry = "0.0";
        private string _inclineEntry = "0.0";
        private double _speed;
        private double _incline;

        public string GeneralStatus
        {
            get => _generalStatus;
            set
            {
                if (value == _generalStatus)
                    return;
                _generalStatus = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (value == _connectionStatus)
                    return;
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public string SpeedEntry
        {
            get => _speedEntry;
            set
            {
                if (value == _speedEntry)
                    return;
                _speedEntry = value;
                OnPropertyChanged();
            }
        }

        public string InclineEntry
        {
            get => _inclineEntry;
            set
            {
                if (value == _inclineEntry)
                    return;
                _inclineEntry = value;
                OnPropertyChanged();
            }
        }

        public double Speed
        {
            get => _speed;
            set
            {
                if (Math.Abs(value - _speed) < 1e-9)
                    return;
                _speed = value;
                OnPropertyChanged();
            }
        }

        public double Incline
        {
            get => _incline;
            set
            {
                if (Math.Abs(value - _incline) < 1e-9)
                    return;
                _incline = value;
                OnPropertyChanged();
            }
        }

        public ICommand HandleStart { private set; get; }
        public ICommand HandleEnd { private set; get; }
        public ICommand HandlePause { private set; get; }
        public ICommand HandleResume { private set; get; }
        public ICommand HandleSpeedUp { private set; get; }
        public ICommand HandleSpeedDown { private set; get; }
        public ICommand HandleInclineUp { private set; get; }
        public ICommand HandleInclineDown { private set; get; }
        public ICommand HandleGoToSpeed { private set; get; }
        public ICommand HandleGoToIncline { private set; get; }

        public MainViewModel()
        {
            Task.Run(() => ServeHttp());
            Task.Run(() => ServeUdp(new IPEndPoint(IPAddress.Parse("192.168.1.151"), 7889), HandleHealthCallback));
            Task.Run(() => ServeUdp(new IPEndPoint(IPAddress.Parse("192.168.1.151"), 7890), ParseJson));
            Task.Run(() => ManageRegistration());
            Task.Run(() => Poll());

            HandleStart = new Command(async () => await Post("start"));
            HandleEnd = new Command(async () => await Post("end"));
            HandlePause = new Command(async () => await Post("pause"));
            HandleResume = new Command(async () => await Post("resume"));
            HandleSpeedUp = new Command(async () => await Post("speed/setpoint", Speed + 0.1));
            HandleSpeedDown = new Command(async () => await Post("speed/setpoint", Speed - 0.1));
            HandleInclineUp = new Command(async () => await Post("incline/setpoint", Incline + 0.5));
            HandleInclineDown = new Command(async () => await Post("incline/setpoint", Incline - 0.5));
            HandleGoToSpeed = new Command(async () => await Post("speed/setpoint", Convert.ToDouble(SpeedEntry)));
            HandleGoToIncline = new Command(async () => await Post("incline/setpoint", Convert.ToDouble(InclineEntry)));
        }

        public class TreadmillMetric
        {
            public int metric { get; set; }
            public string value { get; set; }
        }

        private void ServeUdp(IPEndPoint endpoint, Action<string> callback)
        {
            while (true)
            {
                var data = _udpClient.Receive(ref endpoint);
                _lastCallback = DateTime.UtcNow;
                var message = System.Text.Encoding.UTF8.GetString(data);
                callback(message);
            }
        }

        private void ServeHttp()
        {
            _httpListener.Prefixes.Add(_httpCallback);
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
                GeneralStatus = $"{json}: {e.Message}";
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
            ConnectionStatus = "Disconnected";
        }

        private async void HandleConnected()
        {
            _connected = true;
            ConnectionStatus = "Connected";
            //var response = await Post("callbacks/metrics/http", _httpCallback);
            var response = await Post("callbacks/metrics/udp", _udpCallback);
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
                var response = await Post("callbacks/health", _udpHealthCallback);

                if (response == default)
                {
                    HandleDisconnected();
                    return;
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HandleConnected();
                }
            });
        }

        void Poll()
        {
            while (true)
            {
                PollOnce();
                Thread.Sleep(5*1000);
            }
        }

        async void PollOnce()
        {
            await SafeExecAsync(async () =>
            {
                var speedString = await _client.GetStringAsync($"{_host}/speed/feedback");
                var inclineString = await _client.GetStringAsync($"{_host}/incline/feedback");

                Speed = Convert.ToDouble(speedString);
                Incline = Convert.ToDouble(inclineString);
            });
        }

        private async Task<HttpResponseMessage> Post(string route, object value = null)
        {
            return await SafeExecAsync(async () =>
            {
                var response = await _client
                    .PostAsync($"{_host}/{route}",
                    new StringContent(value != null ? value.ToString() : string.Empty));

                GeneralStatus = await response.Content.ReadAsStringAsync();

                return response;
            });
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
                GeneralStatus = e.Message;
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
                GeneralStatus = e.Message;
                return default;
            }
        }
    }
}
