using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;

namespace Treadmill.Maui.ViewModels;

public interface ISettingsViewModel : IPreferencesProvider
{
}

public class SettingsViewModel : BindableObject, ISettingsViewModel
{
  public string RemoteTreadmillServiceUrl { get => _config.RemoteTreadmillServiceUrl; set => _config.RemoteTreadmillServiceUrl = value; }
  public string GpioClientRemoteUrl { get => _config.GpioClientRemoteUrl; set => _config.GpioClientRemoteUrl = value; }

  public string LocalIp { get => _config.LocalIp; set => _config.LocalIp = value; }
  public string MetricsIp { get => _config.MetricsIp; set => _config.MetricsIp = value; }

  public string ListenUri { get => _config.ListenUri; set => _config.ListenUri = value; }
  public int LocalHttpPort { get => _config.LocalHttpPort; set => _config.LocalHttpPort = value; }
  public string LocalUrl { get => _config.LocalUrl; }
  public int LocalUdpPort { get => _config.LocalUdpPort; set => _config.LocalUdpPort = value; }
  public int LocalUdpHealthPort { get => _config.LocalUdpHealthPort; set => _config.LocalUdpHealthPort = value; }
  public int LocalUdpMetricsPort { get => _config.LocalUdpMetricsPort; set => _config.LocalUdpMetricsPort = value; }

  public string DisplayName { get; set; } = "Settings";

  private readonly ILogService _logger;
  private readonly IPreferencesAdapter _config;

  public SettingsViewModel(ILogService logger, IPreferencesAdapter config)
  {
    _logger = logger;
    _config = config;
    _config.PreferenceChanged += HandlePreferenceChanged;
  }

  private void HandlePreferenceChanged(string name, object value)
  {
    _logger.Add($"Preference {name} changed to {value}");
    OnPropertyChanged(name);
  }
}
