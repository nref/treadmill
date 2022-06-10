using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treadmill.Domain;
using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;
using Treadmill.Models;

namespace Treadmill.Adapters.RemoteTreadmill
{
  public class RemoteTreadmillAdapter : IRemoteTreadmillAdapter
  {
    public event MetricChangedEvent SpeedChanged;
    public event MetricChangedEvent InclineChanged;

    private readonly IConnectionService _connections;
    private readonly IPreferencesAdapter _config;
    private readonly IRemoteTreadmillClient _client;
    private readonly IUdpService _metricsAdapter;
    private readonly IUdpService _healthAdapter;

    private bool _connected = false;
    private readonly DateTime _lastMessage = DateTime.UtcNow;
    private const int _callbackTimeoutSeconds = 15;

    private double _speed;
    private double _incline;

    public double Speed
    {
      get => _speed;
      private set
      {
        if (Math.Abs(value - _speed) < MathExtensions.ZERO)
          return;
        _speed = value;
        SpeedChanged?.Invoke(value);
      }
    }

    public double Incline
    {
      get => _incline;
      private set
      {
        if (Math.Abs(value - _incline) < MathExtensions.ZERO)
          return;
        _incline = value;
        InclineChanged?.Invoke(value);
      }
    }

    public RemoteTreadmillAdapter
    (
        IConnectionService connections,
        IPreferencesAdapter config,
        IRemoteTreadmillClient client,
        IUdpService udpMetrics,
        IUdpService health,
        IHttpService httpMetrics
    )
    {
      _connections = connections;
      _config = config;
      _client = client;
      _metricsAdapter = udpMetrics;
      _healthAdapter = health;

      Task.Run(async () => await httpMetrics.Serve(new Dictionary<string, Action<string>> { ["callbacks/metrics"] = ParseMetrics }));
      Task.Run(async () => await udpMetrics.Serve(ParseMetrics));
      Task.Run(async () => await health.Serve(HandleHealthCallback));
      Task.Run(async () => await ManageRegistration());
      Task.Run(async () => await Poll());
    }

    public async Task<bool> Start() => await SafeExecAsync(async () => await _client.Start());
    public async Task<bool> End() => await SafeExecAsync(async () => await _client.End());
    public async Task<bool> Pause() => await SafeExecAsync(async () => await _client.Pause());
    public async Task<bool> Resume() => await SafeExecAsync(async () => await _client.Resume());
    public async Task<bool> GoToSpeed(double setpoint) => await SafeExecAsync(async () => await _client.GoToSpeed(setpoint));
    public async Task<bool> GoToIncline(double setpoint) => await SafeExecAsync(async () => await _client.GoToIncline(setpoint));

    /// <summary>
    /// Subscribe to metrics changes via HTTP.
    /// </summary>
    /// <param name="ipAndPort">Uri e.g. http://127.0.0.1/path/to/endpoint</param>
    private async Task<bool> AddHttpMetricsCallback(string uri) => await _client.AddHttpMetricsCallback(uri);

    /// <summary>
    /// Subscribe to metrics changes via UDP.
    /// Much faster than HTTP but unreliable
    /// Must be a different port than that set via AddUdpHealthCallback
    /// </summary>
    private async Task<bool> AddUdpMetricsCallback(string ip, int port) => await _client.AddUdpMetricsCallback(ip, port);


    /// <summary>
    /// Subscribe to regular health pings from the server via UDP.
    /// Must be a different port than that set via AddUdpMetricsCallback
    /// </summary>
    private async Task<bool> AddUdpHealthCallback(string ip, int port) => await _client.AddUdpHealthCallback(ip, port);

    private async Task<double> GetSpeedFeedback() => await SafeExecAsync(async () => await _client.GetSpeedFeedback());
    private async Task<double> GetInclineFeedback() => await SafeExecAsync(async () => await _client.GetInclineFeedback());

    private void HandleHealthCallback(string message)
    {

    }

    private void ParseMetrics(string json)
    {
      try
      {
        var obj = JsonConvert.DeserializeObject<TreadmillMetric>(json);

        switch (obj.Metric)
        {
          case Metric.Speed:
            Speed = Convert.ToDouble(obj.Value);
            break;
          case Metric.Incline:
            Incline = Convert.ToDouble(obj.Value);
            break;
        }
      }
      catch (Exception e)
      {
        Log.Add($"{json}: {e.Message}");
      }
    }

    private void HandleDisconnected()
    {
      if (!_connected)
      {
        return;
      }

      _connected = false;
      _connections.NotifyConnectionChanged("Disconnected");
    }

    private async void HandleConnected()
    {
      if (_connected)
      {
        return;
      }

      await Async.SafeExec(async () =>
      {
              //bool ok2 = await AddHttpMetricsCallback(_config.LocalUrl);
        bool ok = await AddUdpMetricsCallback(_config.LocalIp, _config.LocalUdpPort);
        if (ok)
        {
          _connected = true;
          _connections.NotifyConnectionChanged("Connected");
        }
      }, Disconnect);
    }

    private async Task ManageRegistration()
    {
      while (true)
      {
        if (new[]
        {
                    _lastMessage,
                    _healthAdapter.LastMessage,
                    _metricsAdapter.LastMessage
                }.All(x => (DateTime.UtcNow - x).TotalSeconds > _callbackTimeoutSeconds))
        {
          HandleDisconnected();
        }

        if (!_connected)
          await Register();

        await Task.Delay(1000).ConfigureAwait(false);
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

    private async Task Poll()
    {
      while (true)
      {
        await PollOnce();
        await Task.Delay(5 * 1000).ConfigureAwait(false);
      }
    }

    private async Task PollOnce()
    {
      Speed = await SafeExecAsync(async () => await GetSpeedFeedback());
      Incline = await SafeExecAsync(async () => await GetInclineFeedback());
    }

    private async Task SafeExecAsync(Func<Task> f) => await Async.SafeExec(f, Disconnect);
    private async Task<T> SafeExecAsync<T>(Func<Task<T>> f) => await Async.SafeExec(f, Disconnect);

    private void Disconnect(Exception e)
    {
      HandleDisconnected();
      Log.Add(e.Message);
    }
  }
}
