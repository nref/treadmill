namespace Treadmill.Ui.DomainServices
{
    public delegate void ConnectionChangedEvent(string status);

    public interface IConnectionService
    {
        event ConnectionChangedEvent ConnectionChanged;
        void NotifyConnectionChanged(string message);
    }

    public class ConnectionService : IConnectionService
    {
        public event ConnectionChangedEvent ConnectionChanged;
        private readonly ILoggingService _logger;


        public ConnectionService(ILoggingService logger)
        {
            _logger = logger;
        }

        public void NotifyConnectionChanged(string message)
        {
            _logger.LogEvent($"NotifyConnectionChanged({message})");
            ConnectionChanged?.Invoke(message);
        }
    }
}
