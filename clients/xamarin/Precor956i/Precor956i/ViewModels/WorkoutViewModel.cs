using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Precor956i.DomainServices;
using Precor956i.Models;
using Precor956i.Shared;
using Xamarin.Forms;

namespace Precor956i.ViewModels
{
    public interface IWorkoutViewModel
    {

    }

    public class WorkoutViewModel : BindableObject, IWorkoutViewModel
    {
        public bool Paused
        {
            get => _paused; set
            {
                if (_paused == value)
                    return;

                _paused = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName { get; set; } = "Workout";
        public ICommand HandleDoWorkout { private set; get; }
        public ICommand HandlePauseWorkout { private set; get; }
        public ICommand HandleDeleteSegment { private set; get; }
        
        public FullyObservableCollection<WorkoutSegment> Workout { get; set; } = new FullyObservableCollection<WorkoutSegment>();

        private readonly ILoggingService _logger;
        private readonly ITreadmillService _treadmill;
        private bool _paused;
        private DateTime _segmentStart;
        private TimeSpan _pollInterval = new TimeSpan(0, 0, 1);
        private int _segmentIndex;
        private WorkoutSegment _segment { get => _segmentIndex < Workout.Count ? Workout[_segmentIndex] : null; }
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public WorkoutViewModel(ILoggingService logger, ITreadmillService treadmill)
        {
            _logger = logger;
            _treadmill = treadmill;

            ParseWorkout();

            HandleDoWorkout = new Command(DoWorkout);
            HandlePauseWorkout = new Command(PauseWorkout);
            HandleDeleteSegment = new Command(DeleteSegment);
        }

        private void ParseWorkout()
        {
            //string _workout = "960@7.5;45@9.0;45@6.7;45@9.1;45@6.7;45@9.2;45@6.7;45@9.3;45@6.7;45@9.4;45@6.7;45@9.5;45@6.7;400@9.0;960@7.2";
            string _workout = "2700@7.5;20@10.9;20@6.7;20@10.9;20@6.7;20@10.9;20@6.7;20@10.9;20@6.7";
            //string _workout = "3@7.5;2@10.9;2@6.7";

            var segments = _workout.Split(';');
            var workout = segments.Select(segment =>
            {
                var split = segment.Split('@');
                int seconds = Convert.ToInt32(split[0]);
                double speed = Convert.ToDouble(split[1]);

                return new WorkoutSegment
                {
                    Speed = speed,
                    DurationSeconds = seconds
                };
            });

            foreach (var segment in workout)
                Workout.Add(segment);
        }

        private void DeleteSegment(object segment)
        {
            Workout.Remove(segment as WorkoutSegment);
        }

        private async void PauseWorkout()
        {
            _logger.LogEvent($"Un/Pausing workout");
            
            Paused = !Paused;

            if (Paused)
                await _treadmill.Pause();
            else
                await _treadmill.Resume();
        }

        private async void DoWorkout()
        {
            Reset();

            _logger.LogEvent($"Beginning workout");
            await _treadmill.Start();

            DoSegment();
        }

        private async void DoSegment()
        {
            _logger.LogEvent($"Beginning segment {_segmentIndex + 1}");

            _segment.Active = true;
            _segmentStart = DateTime.UtcNow;
            await _treadmill.GoToSpeed(_segment.Speed);

            Device.StartTimer(_pollInterval, () => SegmentTick(_cts.Token));
        }

        private bool SegmentTick(CancellationToken token)
        {
            if (_segmentIndex >= Workout.Count)
            {
                _logger.LogEvent($"Workout finished");
                _treadmill.End();
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
