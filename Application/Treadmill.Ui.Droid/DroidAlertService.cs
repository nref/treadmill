using Android.Widget;
using Treadmill.Domain.Services;

namespace Treadmill.Ui.Droid
{
  public class DroidAlertService : IAlertService
  {
    public void Alert(string message)
    {
      Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
    }
  }
}
