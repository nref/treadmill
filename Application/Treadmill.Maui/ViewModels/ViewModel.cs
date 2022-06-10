using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Treadmill.Maui.ViewModels;

public abstract class ViewModel : BindableObject, INotifyPropertyChanged
{
  protected SynchronizationContext _ui = SynchronizationContext.Current;

  public void NotifyPropertyChanged([CallerMemberName] string name = null) => OnPropertyChanged(name);
}
