using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Treadmill.Maui.Models;

public class Model : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler PropertyChanged;

  public void NotifyPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
