using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Treadmill.Domain.Adapters;

namespace Treadmill.Maui.Adapters;

public class MauiPreferencesAdapter : PreferencesAdapter
{
  protected override T Get<T>([CallerMemberName] string key = "")
  {
    if (!Preferences.ContainsKey(key))
      return default;
    string json = Preferences.Get(key, "");
    return JsonConvert.DeserializeObject<T>(json);
  }

  protected override void Set<T>(T value, [CallerMemberName] string key = "")
  {
    string json = JsonConvert.SerializeObject(value);
    Preferences.Set(key, json);
    NotifyPreferenceChanged(key, value);
  }
}
