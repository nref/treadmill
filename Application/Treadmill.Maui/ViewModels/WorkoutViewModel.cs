using System.Windows.Input;
using Treadmill.Domain.Services;
using Treadmill.Maui.Models;
using Treadmill.Models;

namespace Treadmill.Maui.ViewModels;

public interface IWorkoutViewModel
{
  Workout Workout { get; set; }
  WorkoutState WorkoutState { get; }
}

public class WorkoutViewModel : ViewModel, IWorkoutViewModel
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
      NotifyPropertyChanged();
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

      NotifyPropertyChanged();
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

      NotifyPropertyChanged();
    }
  }

  private readonly IAlertService _alerts;
  private readonly IRemoteTreadmillService _treadmill;
  private DateTime _segmentStart;
  private TimeSpan _pollInterval = new TimeSpan(0, 0, 1);
  private int _segmentIndex;

  /// <summary>
  /// The currently executing segment of the workout
  /// </summary>
  private WorkoutSegment _Segment { get => _segmentIndex < Workout.Count ? Workout[_segmentIndex] : null; }
  private CancellationTokenSource _cts = new CancellationTokenSource();

  public ICommand StartAddCommand { private set; get; }
  public ICommand StartRemoveCommand { private set; get; }
  public ICommand PreviousSegmentCommand { private set; get; }
  public ICommand NextSegmentCommand { private set; get; }
  public ICommand EndWorkoutCommand { private set; get; }
  public ICommand DoWorkoutCommand { private set; get; }
  public ICommand PauseWorkoutCommand { private set; get; }
  public ICommand ResumeWorkoutCommand { private set; get; }
  public ICommand ConfirmRemoveCommand { private set; get; }
  public ICommand CancelRemoveCommand { private set; get; }
  public ICommand FinishAddCommand { private set; get; }
  public ICommand CancelAddCommand { private set; get; }
  
  public WorkoutViewModel(IAlertService alerts, IRemoteTreadmillService treadmill)
  {
    _alerts = alerts;
    _treadmill = treadmill;
    WorkoutState = new WorkoutState(treadmill);
    _treadmill.PropertyChanged += WorkoutState.HandlePropertyChanged;

    StartAddCommand = new Command(HandleStartAdd);
    StartRemoveCommand = new Command(HandleStartRemove);
    PreviousSegmentCommand = new Command(HandlePreviousSegment);
    NextSegmentCommand = new Command(HandleNextSegment);
    EndWorkoutCommand = new Command(() => _ = EndWorkout());
    DoWorkoutCommand = new Command(() => _ = DoWorkout());
    PauseWorkoutCommand = new Command(() => _ = PauseWorkout());
    ResumeWorkoutCommand = new Command(() => _ = ResumeWorkout());
    ConfirmRemoveCommand = new Command(HandleConfirmRemove);
    CancelRemoveCommand = new Command(HandleCancelRemove);
    FinishAddCommand = new Command(HandleFinishAdd);
    CancelAddCommand = new Command(HandleCancelAdd);
  }

  public void HandleStartAdd()
  {
    if (Workout == null)
    {
      return;
    }

    Adding = true;
  }

  public void HandleFinishAdd()
  {
    if (Workout == null)
    {
      return;
    }

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
    if (Workout == null)
    {
      return;
    }

    if (SelectedSegment == default)
    {
      return;
    }

    ConfirmingRemove = true;
  }

  public void HandleConfirmRemove()
  {
    if (Workout == null)
    {
      return;
    }

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
    Log.Add($"Skipping segment");
    NextSegment();
  }

  public void HandlePreviousSegment()
  {
    Log.Add($"Going back to previous segment");
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
      Log.Add($"Cannot start workout; none selected");
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
      Log.Add($"Workout finished");
      await _treadmill.End();
      return true;
    }

    Log.Add($"Beginning segment {_segmentIndex + 1}");

    _Segment.Active = true;
    _segmentStart = DateTime.UtcNow;

    _alerts.Alert($"Going to speed {_Segment.Pace.MinutesPerMileString} and {_Segment.Incline}% incline!");

    bool ok = await _treadmill.GoToIncline(_Segment.Incline);
    ok &= await _treadmill.GoToSpeed(_Segment.Speed);

    StartSegment();

    return ok;
  }

  private void StartSegment()
  {
    var timer = Dispatcher.CreateTimer();
    timer.Interval = _pollInterval;
    timer.Tick += (sender, e) => SegmentTick(_cts.Token);
    timer.Start();
  }

  private bool SegmentTick(CancellationToken token)
  {
    if (!WorkoutState.Active)
      return false;

    if (_segmentIndex >= Workout.Count)
    {
      Log.Add($"Workout finished mid-segment");
      _treadmill.End();
      return false;
    }

    if (token.IsCancellationRequested)
    {
      Log.Add($"Segment cancelled");
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
      Log.Add($"Finished segment {_segmentIndex + 1}");

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
