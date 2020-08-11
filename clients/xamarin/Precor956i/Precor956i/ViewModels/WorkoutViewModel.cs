using System;
using System.Threading;
using System.Windows.Input;
using Precor956i.DomainServices;
using Precor956i.Models;
using Xamarin.Forms;

namespace Precor956i.ViewModels
{
    public interface IWorkoutViewModel
    {
        Workout Workout { get; set; }
    }

    public class WorkoutViewModel : BindableObject, IWorkoutViewModel
    {
        public bool Idle => !Active && !Paused;

        public bool Paused
        {
            get => _paused; 
            set
            {
                if (_paused == value)
                    return;

                _paused = value;
                OnPropertyChanged();
            }
        }

        public bool Active 
        { 
            get => _workoutActive;
            set
            {
                if (_workoutActive == value)
                    return;

                _workoutActive = value;
                OnPropertyChanged();
            }
        }

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

        public string DisplayName { get; set; } = "Workout";
        public ICommand HandleDoWorkout { private set; get; }
        public ICommand HandlePauseWorkout { private set; get; }
        public ICommand HandleResumeWorkout { private set; get; }
        public ICommand HandleEndWorkout { private set; get; }
        public ICommand HandleDeleteSegment { private set; get; }
        
        private bool _paused;
        private bool _workoutActive;
        private Workout _workout;

        private readonly ILoggingService _logger;
        private readonly ITreadmillService _treadmill;
        private DateTime _segmentStart;
        private TimeSpan _pollInterval = new TimeSpan(0, 0, 1);
        private int _segmentIndex;
        private WorkoutSegment _segment { get => _segmentIndex < Workout.Count ? Workout[_segmentIndex] : null; }
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public WorkoutViewModel(ILoggingService logger, ITreadmillService treadmill)
        {
            _logger = logger;
            _treadmill = treadmill;

            HandleDoWorkout = new Command(DoWorkout);
            HandlePauseWorkout = new Command(PauseWorkout);
            HandleResumeWorkout = new Command(ResumeWorkout);
            HandleEndWorkout = new Command(EndWorkout);
            HandleDeleteSegment = new Command(DeleteSegment);
        }

        private void DeleteSegment(object segment)
        {
            Workout.Remove(segment as WorkoutSegment);
        }

        private async void PauseWorkout()
        {
            Paused = true;

            _logger.LogEvent($"Pausing workout");
            await _treadmill.Pause();
        }

        private async void ResumeWorkout()
        {
            Paused = false;

            _logger.LogEvent($"Resuming workout");
            await _treadmill.Resume();
        }

        private async void EndWorkout()
        {
            _logger.LogEvent($"Ending workout");

            await _treadmill.End();
            Paused = false;
            Active = false;
        }

        private async void DoWorkout()
        {
            if (Workout == default)
            {
                _logger.LogEvent($"Cannot start workout; none selected");
                return;
            }

            Reset();

            _logger.LogEvent($"Beginning workout");
            Active = true;
            await _treadmill.Start();

            DoSegment();
        }

        private async void DoSegment()
        {
            if (!Active)
                return;

            if (_segmentIndex >= Workout.Count)
            {
                _logger.LogEvent($"Workout finished");
                await _treadmill.End();
                Active = false;
                return;
            }

            _logger.LogEvent($"Beginning segment {_segmentIndex + 1}");

            _segment.Active = true;
            _segmentStart = DateTime.UtcNow;

            await _treadmill.GoToIncline(_segment.Incline);
            await _treadmill.GoToSpeed(_segment.Speed);

            Device.StartTimer(_pollInterval, () => SegmentTick(_cts.Token));
        }

        private bool SegmentTick(CancellationToken token)
        {
            if (!Active)
                return false;

            if (_segmentIndex >= Workout.Count)
            {
                _logger.LogEvent($"Workout finished mid-segment");
                _treadmill.End();
                Active = false;
                return false;
            }

            if (token.IsCancellationRequested)
            {
                _logger.LogEvent($"Segment cancelled");
                return false;
            }

            if (_paused)
            {
                _segmentStart += _pollInterval;
                return true;
            }

            _segment.ElapsedSeconds = (int)(DateTime.UtcNow - _segmentStart).TotalSeconds;
            bool segmentDone = _segment.ElapsedSeconds >= _segment.DurationSeconds;

            if (segmentDone)
            {
                _logger.LogEvent($"Finished segment {_segmentIndex + 1}");

                _segment.Active = false;
                _segmentIndex++;
                DoSegment();
            }

            return !segmentDone;
        }

        private void Reset()
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            _segmentIndex = 0;

            foreach (var segment in Workout)
            {
                _segment.Active = false;
                segment.ElapsedSeconds = 0;
            }
        }
    }
}
