using System.Linq;
using Precor956i.Models;
using Precor956i.Shared;
using Xamarin.Forms;

namespace Precor956i.ViewModels
{
    public interface IWorkoutsViewModel
    {

    }

    public class WorkoutsViewModel : BindableObject, IWorkoutsViewModel
    {
        private readonly IWorkoutViewModel _workoutViewModel;

        public FullyObservableCollection<Workout> Workouts { get; set; } = new FullyObservableCollection<Workout>();

        public string DisplayName { get; set; } = "Workouts";

        public Workout Workout
        {
            get => _workoutViewModel.Workout;
            set => _workoutViewModel.Workout = value;
        }

        public WorkoutsViewModel(IWorkoutViewModel workoutViewModel)
        {
            _workoutViewModel = workoutViewModel;
            CreateWorkouts();
        }

        private void CreateWorkouts()
        {
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

            var recovery = "00:10:00.000";
            Workouts.Add(new Workout("8x400m")
            {
                WorkoutSegment.MilesAtPace(2.0, "00:08:00.000"),

                WorkoutSegment.MetersAtPace(400, "00:05:30.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:30.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:15.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:15.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:10.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:10.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:05.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MetersAtPace(400, "00:05:00.000"),
                WorkoutSegment.MetersAtPace(400, recovery),

                WorkoutSegment.MilesAtPace(1.0, "00:08:30.000"),
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
        }
    }
}
