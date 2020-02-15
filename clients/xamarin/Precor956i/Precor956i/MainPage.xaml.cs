using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Precor956i
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly string _host = "http://192.168.1.152:8000";
        private readonly string _callback = "http://172.16.1.3:8080/metrics/callback";
        private double _speedFeedback = 0.0;
        private double _inclineFeedback = 0.0;
        private bool _registered = false;

        private static readonly HttpListener _httpListener = new HttpListener();

        public MainPage()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                _httpListener.Prefixes.Add(_callback);
                _httpListener.Start();

                while (true)
                {
                    var context = _httpListener.GetContext();
                    var request = context.Request;
                    var reader = new StreamReader(request.InputStream);
                    ShowMessage(reader.ReadToEnd());
                    reader.Close();

                    var response = context.Response;
                    response.StatusCode = (int)HttpStatusCode.OK;

                    string responseString = "ok";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                }
            });

            Task.Run(() =>
            {
                while (!_registered)
                {
                    Register();
                    Thread.Sleep(1);
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    Update();
                    Thread.Sleep(250);
                }
            });
        }

        async void Register()
        {
            await SafeExecAsync(async () =>
            {
                var response = await Post("metrics/callbacks", _callback);
                if (response.StatusCode == HttpStatusCode.OK)
                    _registered = true;
            });
        }

        async void Update()
        {
            await SafeExecAsync(async () =>
            {
                var speedString = await _client.GetStringAsync($"{_host}/speed/feedback");
                var inclineString = await _client.GetStringAsync($"{_host}/incline/feedback");

                _speedFeedback = Convert.ToDouble(speedString);
                _inclineFeedback = Convert.ToDouble(inclineString);

                Device.BeginInvokeOnMainThread(() =>
                {
                    speedFeedbackLabel.Text = _speedFeedback.ToString();
                    inclineFeedbackLabel.Text = _inclineFeedback.ToString();
                });
            });
        }

        async void HandleSpeedUp(object sender, EventArgs args)
        {
            await Post("speed/setpoint", _speedFeedback + 0.1);
        }

        async void HandleSpeedDown(object sender, EventArgs args)
        {
            await Post("speed/setpoint", _speedFeedback - 0.1);
        }

        async void HandleInclineUp(object sender, EventArgs args)
        {
            await Post("incline/setpoint", _inclineFeedback + 0.5);
        }

        async void HandleInclineDown(object sender, EventArgs args)
        {
            await Post("incline/setpoint", _inclineFeedback - 0.5);
        }

        async void HandleStart(object sender, EventArgs args)
        {
            await Post("start");
        }

        async void HandleEnd(object sender, EventArgs args)
        {
            await Post("end");
        }

        async void HandlePause(object sender, EventArgs args)
        {
            await Post("pause");
        }

        async void HandleResume(object sender, EventArgs args)
        {
            await Post("resume");
        }

        async void HandleGoToSpeed(object sender, EventArgs args)
        {
            await Post("speed/setpoint", Convert.ToDouble(speedEntry.Text));
        }

        async void HandleGoToIncline(object sender, EventArgs args)
        {
            await Post("incline/setpoint", Convert.ToDouble(inclineEntry.Text));
        }

        async Task<HttpResponseMessage> Post(string route, object value = null)
        {
            return await SafeExecAsync(async () =>
            {
                var response = await _client
                    .PostAsync($"{_host}/{route}", 
                    new StringContent(value != null ? value.ToString() : string.Empty));

                var content = await response.Content.ReadAsStringAsync();

                ShowMessage(content);

                return response;
            });
        }

        async Task SafeExecAsync(Func<Task> f)
        {
            try
            {
                await f();
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }
        }

        async Task<T> SafeExecAsync<T>(Func<Task<T>> f)
        {
            try
            {
                return await f();
            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
                return default;
            }
        }

        void ShowMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() => statusLabel.Text = message);
        }
    }
}
