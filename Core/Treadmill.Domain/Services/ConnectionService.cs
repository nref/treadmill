namespace Treadmill.Domain.Services
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
        private readonly ILogService _logger;

        public ConnectionService(ILogService logger)
        {
            _logger = logger;
        }

        public void NotifyConnectionChanged(string message)
        {
            _logger.Add($"NotifyConnectionChanged({message})");
            ConnectionChanged?.Invoke(message);
        }
    }
}
