using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Precor956i.Models
{
    public class WorkoutSegment : INotifyPropertyChanged
    {
        private bool _active;
        private int _elapsedSeconds;

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
                if (Math.Abs(_elapsedSeconds - value) < 1e-9)
                    return;
                _elapsedSeconds = value;

                NotifyPropertyChanged();
            }
        }

        public int DurationSeconds { get; set; }
        public double Speed { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
