using System.Linq;
using Treadmill.Ui.Models;
using Treadmill.Ui.Shared;
using Xamarin.Forms;

namespace Treadmill.Ui.ViewModels
{
    public interface IWorkoutsViewModel
    {
      bool ConfirmingRemove { get; set; }
    }

    public class WorkoutsViewModel : BindableObject, IWorkoutsViewModel
    {
        public IWorkoutViewModel WorkoutViewModel { get; set; }
        public IWorkoutEditViewModel WorkoutEditViewModel { get; }

        public FullyObservableCollection<Workout> Workouts { get; set; } = new FullyObservableCollection<Workout>();

        public string DisplayName { get; set; } = "Workouts";

        public static string NewWorkoutDefaultName = "New Workout";

        private string _newWorkoutName = NewWorkoutDefaultName;
        public string NewWorkoutName
        {
            get => _newWorkoutName;
            set
            {
                if (_newWorkoutName == value)
                {
                    return;
                }
                _newWorkoutName = value;

                OnPropertyChanged();
            }
        }

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

        public Workout Workout
        {
            get => WorkoutViewModel.Workout;
            set => WorkoutViewModel.Workout = value;
        }

        public WorkoutsViewModel(IWorkoutViewModel workoutViewModel, IWorkoutEditViewModel workoutEditViewModel)
        {
            WorkoutViewModel = workoutViewModel;
            WorkoutEditViewModel = workoutEditViewModel;
            CreateWorkouts();
        }

        public void HandleStartAdd()
        {
            Adding = true;
        }

        public void HandleFinishAdd()
        {
            if (NewWorkoutName == default)
            {
                return;
            }

            Adding = false;
            Workouts.Add(new Workout(NewWorkoutName));
            NewWorkoutName = NewWorkoutDefaultName;
        }

        public void HandleCancelAdd()
        {
            Adding = false;
        }

        public void HandleStartRemove()
        {
            if (Workout == default)
            {
                return;
            }

            ConfirmingRemove = true;
        }

        public void HandleConfirmRemove()
        {
            if (Workout == default)
            {
                return;
            }
        
            ConfirmingRemove = false;
            Workouts.Remove(Workout);
        }

        public void HandleCancelRemove()
        {
            ConfirmingRemove = false;
        }

        private void CreateWorkouts()
        {
            Workouts.Add(new Workout("Manual Workout")
            {
                WorkoutSegment.MilesAtPace(999, "00:20:00.000")
            });

            var workout = new Workout("45' easy 4x20s strides")
            {
                WorkoutSegment.MinutesAtPace(45, "00:8:27.000")
            };

            foreach (var _ in Enumerable.Range(0, 4))
            {
                workout.Add(WorkoutSegment.SecondsAtPace(20, "00:5:30.000"));
                workout.Add(WorkoutSegment.SecondsAtPace(20, "00:08:57.000"));
            }
            Workouts.Add(workout);

            var workout2 = new Workout("5mi easy 4x50m light hill strides")
            {
                WorkoutSegment.MilesAtPace(5, "00:8:57.000").AtIncline(1.0)
            };

            foreach (var _ in Enumerable.Range(0, 4))
            {
                workout2.Add(WorkoutSegment.SecondsAtPace(10, "00:6:30.000").AtIncline(4.0));
                workout2.Add(WorkoutSegment.SecondsAtPace(10, "00:08:57.000").AtIncline(4.0));
            }

            Workouts.Add(workout2);

            string abbyJog = "00:12:00.000";
            string abbyEasy = "00:20:00.000";

            Workouts.Add(new Workout("C25K Week 1")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.SecondsAtPace(60, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("C25K Week 2")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("C25K Week 3")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.MinutesAtPace(3, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),

                WorkoutSegment.SecondsAtPace(90, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.MinutesAtPace(3, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("C25K Week 4")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.MinutesAtPace(3, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyJog),
                WorkoutSegment.MinutesAtPace(2.5, abbyEasy),

                WorkoutSegment.MinutesAtPace(3, abbyJog),
                WorkoutSegment.SecondsAtPace(90, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyJog),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("C25K Week 5")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyJog),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("C25K Week 6")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),

                WorkoutSegment.MinutesAtPace(8, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),

                WorkoutSegment.MinutesAtPace(5, abbyJog),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("C25K Week 7")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.MinutesAtPace(12, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),
                WorkoutSegment.MinutesAtPace(12, abbyJog),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("C25K Week 8")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.MinutesAtPace(14, abbyJog),
                WorkoutSegment.MinutesAtPace(3, abbyEasy),
                WorkoutSegment.MinutesAtPace(14, abbyJog),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            Workouts.Add(new Workout("Abby Practice 5km")
            {
                WorkoutSegment.MinutesAtPace(5, abbyEasy),

                WorkoutSegment.MilesAtPace(1, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.MilesAtPace(1, abbyJog),
                WorkoutSegment.MinutesAtPace(2, abbyEasy),

                WorkoutSegment.MilesAtPace(0.9, abbyJog),

                WorkoutSegment.MinutesAtPace(5, abbyEasy),
            });

            var recovery = "00:11:00.000";
            Workouts.Add(new Workout("8x400m")
            {
                // Warmup
                WorkoutSegment.MilesAtPace(2.0, "00:08:00.000"),

                // Strides
                WorkoutSegment.MetersAtPace(100, "00:05:45.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                WorkoutSegment.MetersAtPace(100, "00:05:45.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                WorkoutSegment.MetersAtPace(100, "00:05:45.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                WorkoutSegment.MetersAtPace(100, "00:05:45.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                // rest
                WorkoutSegment.MetersAtPace(100, "00:20:00.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                // Main set
                WorkoutSegment.MetersAtPace(400, "00:05:45.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:45.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:30.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:30.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:15.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:15.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:00.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:00.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                // Cooldown
                WorkoutSegment.MilesAtPace(1.0, "00:10:00.000"),
            });

            Workouts.Add(new Workout("5x1000m")
            {
                WorkoutSegment.MilesAtPace(2.0, "00:08:00.000"),

                WorkoutSegment.KilometersAtPace(1, "00:6:30.000"),
                WorkoutSegment.MetersAtPace(400, "00:09:00.000"),

                WorkoutSegment.KilometersAtPace(1, "00:6:20.000"),
                WorkoutSegment.MetersAtPace(400, "00:09:00.000"),

                WorkoutSegment.KilometersAtPace(1, "00:6:10.000"),
                WorkoutSegment.MetersAtPace(400, "00:09:00.000"),

                WorkoutSegment.KilometersAtPace(1, "00:6:00.000"),
                WorkoutSegment.MetersAtPace(400, "00:09:00.000"),

                WorkoutSegment.KilometersAtPace(1, "00:5:50.000"),
                WorkoutSegment.MetersAtPace(400, "00:09:00.000"),

                WorkoutSegment.MilesAtPace(1.0, "00:08:30.000"),
            });

            Workouts.Add(new Workout("3x2mi")
            {
                WorkoutSegment.MilesAtPace(2.0, "00:08:00.000"),

                WorkoutSegment.MilesAtPace(2.0, "00:6:50.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:08:00.000"),

                WorkoutSegment.MilesAtPace(2.0, "00:6:40.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:08:00.000"),

                WorkoutSegment.MilesAtPace(2.0, "00:6:30.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:08:00.000"),

                WorkoutSegment.MilesAtPace(1.0, "00:08:30.000"),
            });

            Workouts.Add(new Workout("QuickTest")
            {
                WorkoutSegment.SecondsAtPace(2, "00:5:30.000"),
                WorkoutSegment.SecondsAtPace(2, "00:08:00.000"),
                WorkoutSegment.SecondsAtPace(2, "00:5:30.000"),
                WorkoutSegment.SecondsAtPace(1, "00:08:00.000"),
                WorkoutSegment.SecondsAtPace(1, "00:5:30.000"),
                WorkoutSegment.MilesAtPace(0.01, "00:08:00.000"),
            });

            Workouts.Add(new Workout("2up 4x[800m 400jg] 1dn")
            {
                WorkoutSegment.MilesAtPace(2.0, "00:08:57.000"),

                // strides
                WorkoutSegment.MetersAtPace(100, "00:6:00.000"),
                WorkoutSegment.MetersAtPace(100, recovery),
                WorkoutSegment.MetersAtPace(100, "00:6:00.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                // rest
                WorkoutSegment.MetersAtPace(100, "00:20:00.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                // main set
                WorkoutSegment.MetersAtPace(800, "00:6:00.000"),
                WorkoutSegment.MetersAtPace(400, recovery),
                WorkoutSegment.MetersAtPace(800, "00:5:55.000"),
                WorkoutSegment.MetersAtPace(400, recovery),
                WorkoutSegment.MetersAtPace(800, "00:5:50.000"),
                WorkoutSegment.MetersAtPace(400, recovery),
                WorkoutSegment.MetersAtPace(800, "00:5:45.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MilesAtPace(1.0, "00:10:00.000"),
            });

            Workouts.Add(new Workout("2up 2x[1mi 400jg] 1dn")
            {
                WorkoutSegment.MilesAtPace(2.0, "00:08:57.000"),

                // strides
                WorkoutSegment.MetersAtPace(100, "00:6:00.000"),
                WorkoutSegment.MetersAtPace(100, recovery),
                WorkoutSegment.MetersAtPace(100, "00:6:00.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                // rest
                WorkoutSegment.MetersAtPace(100, "00:20:00.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                // main set
                WorkoutSegment.MilesAtPace(1, "00:6:00.000"),
                WorkoutSegment.MetersAtPace(400, recovery),
                WorkoutSegment.MilesAtPace(1, "00:6:00.000"),

                WorkoutSegment.MilesAtPace(1.0, "00:10:00.000"),
            });

            Workouts.Add(new Workout("7mi light progression")
            {
                WorkoutSegment.MilesAtPace(1.0, "00:08:57.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:08:49.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:08:34.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:08:20.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:08:06.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:07:54.000"),
                WorkoutSegment.MilesAtPace(1.0, "00:07:42.000"),
            });

            Workouts.Add(new Workout("1up 2x[1mi 400jg]")
            {
                WorkoutSegment.MilesAtPace(1.0, "00:08:00.000"),

                // strides
                WorkoutSegment.MetersAtPace(100, "00:6:30.000"),
                WorkoutSegment.MetersAtPace(100, recovery),
                WorkoutSegment.MetersAtPace(100, "00:6:20.000"),
                WorkoutSegment.MetersAtPace(100, recovery),

                // main set
                WorkoutSegment.MilesAtPace(1, "00:6:30.000"),
                WorkoutSegment.MetersAtPace(400, recovery),
                WorkoutSegment.MilesAtPace(1, "00:6:20.000"),
            });

            Workouts.Add(new Workout("1up 1mi 1dn")
            {
                WorkoutSegment.MilesAtPace(1.0, "00:08:20.000").AtIncline(1),

                // strides
                WorkoutSegment.MetersAtPace(100, "00:6:15.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(100, recovery),
                WorkoutSegment.MetersAtPace(100, "00:6:00.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(100, recovery),

                // main set
                WorkoutSegment.MilesAtPace(1, "00:6:00.000").AtIncline(1),

                WorkoutSegment.MilesAtPace(1.0, "00:08:57.000").AtIncline(1),
            });

            Workouts.Add(new Workout("1up 4x[800m 400jg]")
            {
                WorkoutSegment.MilesAtPace(1.0, "00:08:00.000").AtIncline(1),

                // strides
                WorkoutSegment.MetersAtPace(100, "00:6:00.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(1),
                WorkoutSegment.MetersAtPace(100, "00:5:46.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(1),

                // main set
                WorkoutSegment.MetersAtPace(800, "00:5:53.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(400, recovery).AtIncline(1),
                WorkoutSegment.MetersAtPace(800, "00:5:46.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(400, recovery).AtIncline(1),
                WorkoutSegment.MetersAtPace(800, "00:5:40.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(400, recovery).AtIncline(1),
                WorkoutSegment.MetersAtPace(800, "00:5:30.000").AtIncline(1),
                WorkoutSegment.MetersAtPace(400, recovery).AtIncline(1),
            });

            Workouts.Add(new Workout("long reps uphill")
            {
                // warm up
                WorkoutSegment.MilesAtPace(2.0, "00:09:30.000").AtIncline(1),

                // strides
                WorkoutSegment.MetersAtPace(100, "00:9:45.000").AtIncline(10),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(10),
                WorkoutSegment.MetersAtPace(100, "00:9:30.000").AtIncline(10),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(10),
                WorkoutSegment.MetersAtPace(100, "00:9:15.000").AtIncline(10),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(10),
                WorkoutSegment.MetersAtPace(100, "00:9:00.000").AtIncline(10),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(10),

                // main set
                WorkoutSegment.MetersAtPace(500, "00:9:00.000").AtIncline(10),
                WorkoutSegment.MinutesAtPace(5, "00:18:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(500, "00:9:00.000").AtIncline(10),
                WorkoutSegment.MinutesAtPace(5, "00:18:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(500, "00:9:00.000").AtIncline(10),
                WorkoutSegment.MinutesAtPace(5, "00:18:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(500, "00:9:00.000").AtIncline(10),
                WorkoutSegment.MinutesAtPace(5, "00:18:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(500, "00:9:00.000").AtIncline(10),
                WorkoutSegment.MinutesAtPace(5, "00:18:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(500, "00:9:00.000").AtIncline(10),
                WorkoutSegment.MinutesAtPace(5, "00:18:00.000").AtIncline(0),

                // cool down
                WorkoutSegment.MilesAtPace(1.0, "00:10:00.000").AtIncline(0),
            });

            Workouts.Add(new Workout("14x100m 800m")
            {
                // warm up
                WorkoutSegment.MilesAtPace(2.0, "00:09:30.000").AtIncline(1),

                // strides
                WorkoutSegment.MetersAtPace(100, "00:8:00.000").AtIncline(5),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(5),
                WorkoutSegment.MetersAtPace(100, "00:7:45.000").AtIncline(5),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(5),
                WorkoutSegment.MetersAtPace(100, "00:7:30.000").AtIncline(5),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(5),
                WorkoutSegment.MetersAtPace(100, "00:7:15.000").AtIncline(5),
                WorkoutSegment.MetersAtPace(100, recovery).AtIncline(5),

                // main set
                // 14x100
                WorkoutSegment.MetersAtPace(100, "00:7:30.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:7:30.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:7:20.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:7:20.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:7:10.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:7:10.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:7:00.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),

                WorkoutSegment.MetersAtPace(100, "00:7:00.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:6:50.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:6:50.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:6:40.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:6:40.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:6:30.000").AtIncline(5),
                WorkoutSegment.SecondsAtPace(45, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(100, "00:6:30.000").AtIncline(5),

                // 800
                WorkoutSegment.MinutesAtPace(2, "00:11:00.000").AtIncline(0),
                WorkoutSegment.MetersAtPace(800, "00:5:30.000").AtIncline(1),

                // cool down
                WorkoutSegment.MilesAtPace(2.0, "00:10:00.000").AtIncline(0),
            });
        }
    }
}
