namespace Treadmill.Maui.Models;

public class LogEntry : Model
{
  private string _message;

  public string Message
  {
    get => _message; set
    {
      _message = value;
      NotifyPropertyChanged();
    }
  }
}
