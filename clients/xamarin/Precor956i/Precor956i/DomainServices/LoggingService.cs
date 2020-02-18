namespace Precor956i.DomainServices
{
    public delegate void MessageLoggedEvent(string message);

    public interface ILoggingService
    {
        event MessageLoggedEvent EventLogged;
        void Log(string message);
    }

    public class LoggingService : ILoggingService
    {
        public event MessageLoggedEvent EventLogged;

        public void Log(string message)
        {
            EventLogged?.Invoke(message);
        }
    }
}
