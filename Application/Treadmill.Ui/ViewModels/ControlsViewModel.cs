using System;
using System.Windows.Input;
using Xamarin.Forms;
using Treadmill.Models;
using Treadmill.Domain.Adapters;
using Treadmill.Domain.Services;

namespace Treadmill.Ui.ViewModels
{
    public interface IControlsViewModel
    {

    }

    public class ControlsViewModel : BindableObject, IControlsViewModel
    {
        private readonly IRemoteTreadmillAdapter _service;
        private readonly IConnectionService _connections;

        private string _connectionStatus = "---";
        private string _speedEntry = "0.0";
        private string _inclineEntry = "0.0";
        private double _speed;
        private double _incline;

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
                if (Math.Abs(value - _speed) < MathExtensions.ZERO)
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
                if (Math.Abs(value - _incline) < MathExtensions.ZERO)
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

        public string DisplayName { get; set; } = "Controls";

        public ControlsViewModel(IConnectionService connections, IRemoteTreadmillAdapter service)
        {
            _connections = connections;
            _service = service;

            _connections.ConnectionChanged += HandleConnectionChanged;
            service.SpeedChanged += HandleSpeedChanged;
            service.InclineChanged += HandleInclineChanged;

            HandleStart = new Command(async () => await _service.Start());
            HandleEnd = new Command(async () => await _service.End());
            HandlePause = new Command(async () => await _service.Pause());
            HandleResume = new Command(async () => await _service.Resume());
            HandleSpeedUp = new Command(async () => await _service.GoToSpeed(Speed + 0.1));
            HandleSpeedDown = new Command(async () => await _service.GoToSpeed(Speed - 0.1));
            HandleInclineUp = new Command(async () => await _service.GoToIncline(Incline + 0.5));
            HandleInclineDown = new Command(async () => await _service.GoToIncline(Incline - 0.5));
            HandleGoToSpeed = new Command(async () => await _service.GoToSpeed(Convert.ToDouble(SpeedEntry)));
            HandleGoToIncline = new Command(async () => await _service.GoToIncline(Convert.ToDouble(InclineEntry)));
        }

        private void HandleInclineChanged(double value)
        {
            Incline = value;
        }

        private void HandleSpeedChanged(double value)
        {
            Speed = value;
        }

        private void HandleConnectionChanged(string status)
        {
            ConnectionStatus = status;
        }
    }
}
