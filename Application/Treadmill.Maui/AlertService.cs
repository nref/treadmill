using Treadmill.Domain.Services;

#if __ANDROID__
using Android.Widget;
#endif

namespace Treadmill.Maui;

public class AlertService : IAlertService
{
  public void Alert(string message)
  {
    if (DeviceInfo.Platform == DevicePlatform.Android)
    {
#if __ANDROID__
      Toast.MakeText(Android.App.Application.Context, message, ToastLength.Long).Show();
#endif
    }
  }
}
