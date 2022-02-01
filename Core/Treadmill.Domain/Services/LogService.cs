using System;

namespace Treadmill.Domain.Services
{
  public delegate void MessageLoggedEvent(string message);

  public interface ILogService
  {
    event MessageLoggedEvent EventLogged;
    void Add(string message);
  }

  public class LogService : ILogService
  {
    public event MessageLoggedEvent EventLogged;

    public void Add(string message)
    {
      string msg = $"{DateTime.Now}: {message}\n";
      Console.Write(msg);
      EventLogged?.Invoke(msg);
    }
  }

}
