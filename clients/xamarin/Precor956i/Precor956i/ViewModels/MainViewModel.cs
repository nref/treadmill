using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Precor956i.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly string _host = "http://192.168.1.152:8000";
        private readonly string _callback = "http://172.16.1.3:8080/metrics/callback/";
        private bool _connected = false;
        private DateTime _lastCallback = DateTime.UtcNow;
        private const int CallbackTimeoutSeconds = 5;

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
            Task.Run(() => Serve());
            Task.Run(() => ManageRegistration());
            //Task.Run(() => Poll());

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

        private void Serve()
        {
            _httpListener.Prefixes.Add(_callback);
            _httpListener.Start();

            while (true)
            {
                var context = _httpListener.GetContext();
                _lastCallback = DateTime.UtcNow;
                var json = Read(context.Request);

                if (context.Request.Url.AbsoluteUri.EndsWith("metrics/callback/"))
                {
                    Parse(json);
                }

                Write(context.Response, HttpStatusCode.OK, "ok");
            }
        }

        private void Parse(string json)
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
                GeneralStatus = e.Message;
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

        private void HandleConnected()
        {
            _connected = true;
            ConnectionStatus = "Connected";
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
                var response = await Post("metrics/callbacks", _callback);
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
                Thread.Sleep(250);
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
