using Precor956i.DomainServices;
using Precor956i.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Precor956i.ViewModels
{
    public interface ILogViewModel
    {

    }

    public class LogViewModel : BindableObject, ILogViewModel
    {
        private ObservableCollection<LogEntry> _log = new ObservableCollection<LogEntry>();

        public ObservableCollection<LogEntry> Log
        {
            get => _log;
            set
            {
                if (value == _log)
                    return;
                _log = value;
                OnPropertyChanged();
            }
        }
        public string DisplayName { get; set; } = "Log";
        
        private readonly ILoggingService _logger;

        public LogViewModel(ILoggingService logger)
        {
            _logger = logger;
            _logger.EventLogged += HandleEventLogged;
            _logger = logger;

            HandleEventLogged("Logger ready");
        }

        private void HandleEventLogged(string message)
        {
            Log.Insert(0, new LogEntry { Message = $"{DateTime.Now}: {message}\n" });
        }
    }
}
