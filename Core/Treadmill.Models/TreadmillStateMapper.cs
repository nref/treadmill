using System;

namespace Treadmill.Models
{
  public static class TreadmillStateMapper
  {
    public static TreadmillState Map(string s) => Enum.TryParse(s, out TreadmillState state)
      ? state
      : TreadmillState.Unknown;
  }
}
