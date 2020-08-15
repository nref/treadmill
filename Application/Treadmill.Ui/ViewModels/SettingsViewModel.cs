using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;
using Xamarin.Forms;

namespace Treadmill.Ui.ViewModels
{
    public interface ISettingsViewModel : IPreferencesProvider
    {

    }

    public class SettingsViewModel : BindableObject, ISettingsViewModel
    {
        public string RemoteUrl { get => _config.RemoteUrl; set => _config.RemoteUrl = value; }
        public string LocalIp { get => _config.LocalIp; set => _config.LocalIp = value; }
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
}
