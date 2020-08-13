using Treadmill.Ui.DomainServices;
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

        public string DisplayName { get; set; } = "Settings";

        private readonly ILoggingService _logger;
        private readonly IPreferencesService _config;

        public SettingsViewModel(ILoggingService logger, IPreferencesService config)
        {
            _logger = logger;
            _config = config;
            _config.PreferenceChanged += HandlePreferenceChanged;
        }

        private void HandlePreferenceChanged(string name, object value)
        {
            _logger.LogEvent($"Preference {name} changed to {value}");
            OnPropertyChanged(name);
        }
    }
}
