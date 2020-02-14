using System;
using System.ComponentModel;
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
        private double _speedFeedback = 0.0;
        private double _inclineFeedback = 0.0;

        public MainPage()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(250);
                    Update();
                }
            });
        }

        async void Update()
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

        async Task Post(string route, object value = null)
        {
            await SafeExecAsync(async () =>
            {
                var response = await _client
                    .PostAsync($"{_host}/{route}", 
                    new StringContent(value != null ? value.ToString() : string.Empty));

                var content = await response.Content.ReadAsStringAsync();

                ShowMessage(content);
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

        void ShowMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() => statusLabel.Text = message);
        }
    }
}
