using Treadmill.Models;

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

    public void NotifyConnectionChanged(string message)
    {
      Log.Add($"{nameof(NotifyConnectionChanged)}({message})");
      ConnectionChanged?.Invoke(message);
    }
  }
}
