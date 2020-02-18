namespace Precor956i.DomainServices
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

        public void NotifyConnectionChanged(string message)
        {
            ConnectionChanged?.Invoke(message);
        }
    }
}
