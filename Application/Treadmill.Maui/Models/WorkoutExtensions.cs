namespace Treadmill.Maui.Models;

public static class WorkoutExtensions
{
  public static Workout Then(this Workout w, params WorkoutSegment[] segments)
  {
    foreach (var segment in segments)
    {
      w.Add(segment);
    }
    return w;
  }

  public static Workout AtIncline(this Workout w, double incline)
  {
    foreach (var segment in w)
    {
      segment.AtIncline(incline);
    }
    return w;
  }
}
