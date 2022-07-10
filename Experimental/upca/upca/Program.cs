using System.IO.Ports;

public static class Program
{
  public static void Main()
  {
    using var reader = new UpcaReader("COM6");
    var parser = new UpcaParser();

    _ = Task.Run(async () =>
    {
      while (true)
      {
        reader.Write("[ver]");
        await Task.Delay(1000);
      }
    });

    string buffer = "";

    while (reader.IsOpen)
    {
      string s = reader.ReadAll();
      Console.Write(s);

      //buffer += s;

      //if (parser.TryParse(buffer, out string? result))
      //{
      //  buffer = buffer.Replace($"[{result}]", "");
      //  Console.WriteLine(result);
      //}
    }
  }
}

public class UpcaReader : IDisposable
{
  private readonly SerialPort _port;
  public bool IsOpen => _port.IsOpen;

  public UpcaReader(string portName)
  {
    _port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One)
    {
      Encoding = System.Text.Encoding.ASCII
    };
    _port.Open();
  }

  public void Write(string message) => _port.Write(message);
  public string ReadAll() => _port.ReadExisting();
  public void Dispose() => _port.Dispose();
}

public class UpcaParser
{
  /// <summary>
  /// Given the message buffer from the UPCA or LPCA,
  /// return the first string between [ and ].
  /// Return false if [ or ] were not found in the buffer.
  /// 
  /// Remove the square brackets and returned string from the buffer.
  /// </summary>
  public bool TryParse(string buffer, out string? result)
  {
    int open = buffer.IndexOf("[");
    int close = buffer.IndexOf("]");

    if (open < 0 || close < 0)
    {
      result = null;
      return false;
    }

    result = buffer[(open + 1)..close];
    return true;
  }
}