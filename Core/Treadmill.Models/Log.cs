using System;

namespace Treadmill.Models;

public delegate void MessageLoggedEvent(string message);

public class Log
{
  public static event MessageLoggedEvent Added;

  public static void Add(string message) => Added(message);
}
