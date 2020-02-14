class TreadmillState:

    # The treadmill is not running
    Ready = 1

    # The 3, 2, 1 start countdown is in progress
    Starting = 2

    # The treadmill is running
    Started = 3

    # The treadmill is paused (could still be rolling)
    Paused = 4

    # Workout has ended, summary is shown
    Summary = 5