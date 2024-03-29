﻿using Treadmill.Ui.Shared;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Treadmill.Models;

namespace Treadmill.Ui.Models
{
    [DebuggerDisplay("{MinutesPerMileString}")]
    public class Pace
    {
        public double MinutesPerMile { get; set; }

        public static Pace FromMinutesPerMile(string pace)
        {
            return new Pace { MinutesPerMile = TimeSpan.Parse(pace).TotalMinutes };
        }

        public string MinutesPerMileString => TimeSpan.FromMinutes(MinutesPerMile).ToString("g");

        public override string ToString()
        {
            return MinutesPerMileString;
        }

        public double ToSpeed()
        {
            return 60 / MinutesPerMile; 
        }

        public static Pace FromSpeed(double speed)
        {
            return new Pace { MinutesPerMile = 60 / speed };
        }
    }

    public class Workout : FullyObservableCollection<WorkoutSegment>
    {
        public string Name { get; set; }

        public Workout(string name = default)
        {
            Name = name;
        }
    }

    public class WorkoutSegment : INotifyPropertyChanged
    {
        private bool _active;
        private int _elapsedSeconds;
        private double _speed;
        private double _incline;
        private double _distanceMiles;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Active
        {
            get => _active;
            set
            {
                if (_active == value)
                    return;
                _active = value;

                NotifyPropertyChanged();
            }
        }

        public int ElapsedSeconds
        {
            get => _elapsedSeconds;
            set
            {
                if (Math.Abs(_elapsedSeconds - value) < MathExtensions.ZERO)
                    return;
                _elapsedSeconds = value;

                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(PercentComplete));
                NotifyPropertyChanged(nameof(TimeRemaining));
      }
        }

        public TimeSpan TimeRemaining => TimeSpan.FromSeconds(DurationSeconds-ElapsedSeconds);
        public double PercentComplete => ElapsedSeconds / (double)DurationSeconds;
        public double DistanceMiles
        {
            get => _distanceMiles;
            set
            {
                if (Math.Abs(_distanceMiles - value) < MathExtensions.ZERO)
                    return;
                _distanceMiles = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Min/mi
        /// </summary>
        public Pace Pace { get => Pace.FromSpeed(Speed); set => Speed = value.ToSpeed(); }

        public int DurationSeconds { get => (int)(60 * 60 * DistanceMiles / Speed); }
        public double Speed
        {
            get => _speed;
            set
            {
                if (Math.Abs(_speed - value) < MathExtensions.ZERO)
                    return;
                _speed = Math.Round(value, 1);
                NotifyPropertyChanged();
            }
        }

        public double Incline
        {
            get => _incline;
            set
            {
                if (Math.Abs(_incline - value) < MathExtensions.ZERO)
                    return;
                _incline = value;
                NotifyPropertyChanged();
            }
        }

        public static WorkoutSegment MetersAtPace(double distance, string paceMinPerMi)
        {
            return new WorkoutSegment { DistanceMiles = distance * 0.000621371 }.AtPace(paceMinPerMi);
        }

        public static WorkoutSegment KilometersAtPace(double distance, string paceMinPerMi)
        {
            return new WorkoutSegment { DistanceMiles = distance * 0.62171 }.AtPace(paceMinPerMi);
        }

        public static WorkoutSegment MilesAtPace(double distance, string paceMinPerMi)
        {
            return new WorkoutSegment { DistanceMiles = distance }.AtPace(paceMinPerMi);
        }

        public static WorkoutSegment MinutesAtPace(double minutes, string paceMinPerMi)
        {
            return DurationAtPace(TimeSpan.FromMinutes(minutes), paceMinPerMi);
        }

        public static WorkoutSegment SecondsAtPace(double seconds, string paceMinPerMi)
        {
            return DurationAtPace(TimeSpan.FromSeconds(seconds), paceMinPerMi);
        }

        public static WorkoutSegment DurationAtPace(TimeSpan duration, string paceMinPerMi)
        {
            return new WorkoutSegment()
                .AtPace(paceMinPerMi)
                .WithDuration(duration.TotalSeconds);
        }

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public static class WorkoutSegmentExtensions
    {
        public static WorkoutSegment AtPace(this WorkoutSegment segment, string pace)
        {
            segment.Pace = Pace.FromMinutesPerMile(pace);
            return segment;
        }

        public static WorkoutSegment WithDuration(this WorkoutSegment segment, double durationSeconds)
        {
            segment.DistanceMiles = durationSeconds / (segment.Pace.MinutesPerMile * 60); // s / (s/mi) == mi
            return segment;
        }

        public static WorkoutSegment AtIncline(this WorkoutSegment segment, double incline)
        {
            segment.Incline = incline;
            return segment;
        }
    }
}
