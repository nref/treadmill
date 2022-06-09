using Treadmill.Domain.Services;
using Treadmill.Maui.Models;

namespace Treadmill.Maui.ViewModels;

public interface IWorkoutViewModel
{
  Workout Workout { get; set; }
  WorkoutState WorkoutState { get; }
}

public class WorkoutViewModel : BindableObject, IWorkoutViewModel
{
  private Workout _workout;
  public Workout Workout
  {
    get => _workout;
    set
    {
      if (_workout == value)
        return;

      _workout = value;
      OnPropertyChanged();
    }
  }

  public WorkoutState WorkoutState { get; }

  /// <summary>
  /// The segment the user selected
  /// </summary>
  public WorkoutSegment SelectedSegment { get; set; }

  /// <summary>
  /// The segment that wil be added
  /// </summary>
  public WorkoutSegment SegmentToAdd { get; set; } = new WorkoutSegment();

  public string DisplayName { get; set; } = "Workout";

  private bool _adding;
  public bool Adding
  {
    get => _adding;
    set
    {
      if (_adding == value)
      {
        return;
      }
      _adding = value;

      OnPropertyChanged();
    }
  }

  private bool _confirmingDelete;
  public bool ConfirmingRemove
  {
    get => _confirmingDelete;
    set
    {
      if (_confirmingDelete == value)
      {
        return;
      }
      _confirmingDelete = value;

      OnPropertyChanged();
    }
  }

  private readonly IAlertService _alerts;
  private readonly ILogService _logger;
  private readonly IRemoteTreadmillService _treadmill;
  private DateTime _segmentStart;
  private TimeSpan _pollInterval = new TimeSpan(0, 0, 1);
  private int _segmentIndex;

  /// <summary>
  /// The currently executing segment of the workout
  /// </summary>
  private WorkoutSegment _Segment { get => _segmentIndex < Workout.Count ? Workout[_segmentIndex] : null; }
  private CancellationTokenSource _cts = new CancellationTokenSource();

  public WorkoutViewModel(IAlertService alerts, ILogService logger, IRemoteTreadmillService treadmill)
  {
    _alerts = alerts;
    _logger = logger;
    _treadmill = treadmill;
    WorkoutState = new WorkoutState(treadmill);
    _treadmill.PropertyChanged += WorkoutState.HandlePropertyChanged;
  }

  public void HandleStartAdd()
  {
    Adding = true;
  }

  public void HandleFinishAdd()
  {
    Adding = false;
    Workout.Add(SegmentToAdd);
    SegmentToAdd = new WorkoutSegment();
  }

  public void HandleCancelAdd()
  {
    Adding = false;
  }

  public void HandleStartRemove()
  {
    if (SelectedSegment == default)
    {
      return;
    }

    ConfirmingRemove = true;
  }

  public void HandleConfirmRemove()
  {
    if (SelectedSegment == default)
    {
      return;
    }

    ConfirmingRemove = false;
    Workout.Remove(SelectedSegment);
  }

  public void HandleCancelRemove()
  {
    ConfirmingRemove = false;
  }

  public void HandleNextSegment()
  {
    _logger.Add($"Skipping segment");
    NextSegment();
  }

  public void HandlePreviousSegment()
  {
    _logger.Add($"Going back to previous segment");
    PreviousSegment();
  }

  public async Task<bool> PauseWorkout() => await _treadmill.Pause();
  public async Task<bool> ResumeWorkout() => await _treadmill.Resume();
  public async Task<bool> EndWorkout()
  {
    Reset();
    return await _treadmill.End();
  }

  public async Task<bool> DoWorkout()
  {
    if (Workout == default)
    {
      _logger.Add($"Cannot start workout; none selected");
      return false;
    }

    Reset();

    bool ok = await _treadmill.Start();

    return ok && await DoSegment();
  }

  private async Task<bool> DoSegment()
  {
    if (!WorkoutState.Active)
    {
      return false;
    }

    if (_segmentIndex >= Workout.Count)
    {
      _logger.Add($"Workout finished");
      await _treadmill.End();
      return true;
    }

    _logger.Add($"Beginning segment {_segmentIndex + 1}");

    _Segment.Active = true;
    _segmentStart = DateTime.UtcNow;

    _alerts.Alert($"Going to speed {_Segment.Pace.MinutesPerMileString} and {_Segment.Incline}% incline!");

    bool ok = await _treadmill.GoToIncline(_Segment.Incline);
    ok &= await _treadmill.GoToSpeed(_Segment.Speed);

    Device.StartTimer(_pollInterval, () => SegmentTick(_cts.Token));

    return ok;
  }

  private bool SegmentTick(CancellationToken token)
  {
    if (!WorkoutState.Active)
      return false;

    if (_segmentIndex >= Workout.Count)
    {
      _logger.Add($"Workout finished mid-segment");
      _treadmill.End();
      return false;
    }

    if (token.IsCancellationRequested)
    {
      _logger.Add($"Segment cancelled");
      return false;
    }

    if (WorkoutState.Paused)
    {
      _segmentStart += _pollInterval;
      return true;
    }

    _Segment.ElapsedSeconds = (int)(DateTime.UtcNow - _segmentStart).TotalSeconds;
    bool segmentDone = _Segment.ElapsedSeconds >= _Segment.DurationSeconds;

    if (segmentDone)
    {
      _logger.Add($"Finished segment {_segmentIndex + 1}");

      NextSegment();
    }

    return !segmentDone;
  }

  private void PreviousSegment()
  {
    if (_segmentIndex == 0)
    {
      return;
    }

    _Segment.Active = false;
    _segmentIndex--;
    _ = DoSegment();
  }

  private void NextSegment()
  {
    _Segment.Active = false;
    _segmentIndex++;
    _ = DoSegment();
  }

  private void Reset()
  {
    _cts.Cancel();
    _cts = new CancellationTokenSource();
    _segmentIndex = 0;

    foreach (var segment in Workout)
    {
      _Segment.Active = false;
      segment.ElapsedSeconds = 0;
    }
  }
}
