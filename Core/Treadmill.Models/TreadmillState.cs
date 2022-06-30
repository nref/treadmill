namespace Treadmill.Models
{
  public enum TreadmillState
  {
    /// <summary>
    /// The treadmill is not running
    /// </summary>
    Ready = 1,

    /// <summary>
    /// The 3, 2, 1 start countdown is in progress
    /// </summary>
    Starting = 2,

    /// <summary>
    /// The treadmill is running
    /// </summary>
    Started = 3,

    /// <summary>
    /// The treadmill is paused (could still be rolling)
    /// </summary>
    Paused = 4,

    /// <summary>
    /// Workout has ended, summary is shown
    /// </summary>
    Summary = 5,
  }
}
