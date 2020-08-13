namespace Treadmill.Ui.DomainServices
{
    public delegate void MessageLoggedEvent(string message);

    public interface ILoggingService
    {
        event MessageLoggedEvent EventLogged;
        event MessageLoggedEvent StatusLogged;
        void LogEvent(string message);
        void LogStatus(string message);
    }

    public class LoggingService : ILoggingService
    {
        public event MessageLoggedEvent EventLogged;
        public event MessageLoggedEvent StatusLogged;

        public void LogEvent(string message)
        {
            EventLogged?.Invoke(message);
        }

        public void LogStatus(string message)
        {
            StatusLogged?.Invoke(message);
        }
    }
}
