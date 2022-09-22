using ElevatorSystemSimulation;

namespace UI
{
    public class ElevatorViewModel : ViewModelBase<Elevator>
    {
        public Centimeters Location { get; set; } 
        public int PeopleCount { get; set; }

        public ElevatorViewModel(Elevator elevator)
        : base(elevator)
        {
            Location = elevator.Location;
            PeopleCount = elevator.AttendingRequests.Count;
        }
    }
}
