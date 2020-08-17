﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Treadmill.Domain.Services;
using Treadmill.Ui.Models;
using Xamarin.Forms;

namespace Treadmill.Ui.ViewModels
{
    public interface IWorkoutViewModel
    {
        Workout Workout { get; set; }
    }

    public class WorkoutViewModel : BindableObject, IWorkoutViewModel
    {
        public WorkoutState WorkoutState { get; }

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
        public ICommand HandleDeleteSegment { private set; get; }
        
        private Workout _workout;

        private readonly ILogService _logger;
        private readonly IRemoteTreadmillService _treadmill;
        private DateTime _segmentStart;
        private TimeSpan _pollInterval = new TimeSpan(0, 0, 1);
        private int _segmentIndex;
        private WorkoutSegment Segment { get => _segmentIndex < Workout.Count ? Workout[_segmentIndex] : null; }
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public WorkoutViewModel(ILogService logger, IRemoteTreadmillService treadmill)
        {
            _logger = logger;
            _treadmill = treadmill;
            HandleDeleteSegment = new Command(DeleteSegment);
            WorkoutState = new WorkoutState(treadmill);
            _treadmill.PropertyChanged += WorkoutState.HandlePropertyChanged;
        }

        public void HandleItemSelected(ListView source, WorkoutSegment selection)
        {
            source.ScrollTo(selection, ScrollToPosition.MakeVisible, true);
        }

        private void DeleteSegment(object segment) => Workout.Remove(segment as WorkoutSegment);

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

            Segment.Active = true;
            _segmentStart = DateTime.UtcNow;

            bool ok = await _treadmill.GoToIncline(Segment.Incline);
            ok &= await _treadmill.GoToSpeed(Segment.Speed);

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

            Segment.ElapsedSeconds = (int)(DateTime.UtcNow - _segmentStart).TotalSeconds;
            bool segmentDone = Segment.ElapsedSeconds >= Segment.DurationSeconds;

            if (segmentDone)
            {
                _logger.Add($"Finished segment {_segmentIndex + 1}");

                Segment.Active = false;
                _segmentIndex++;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                DoSegment();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
                Segment.Active = false;
                segment.ElapsedSeconds = 0;
            }
        }
    }
}
