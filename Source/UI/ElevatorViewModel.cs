using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.Generic;

namespace UI
{
    public class ElevatorViewModel : ViewModelBase<Elevator>, ICloneable
    {
        public int Id { get; }
        public Centimeters Location { get; set; } 
        public int PeopleCount { get; set; }
        public Direction Direction { get; set; }
        public List<IRequestEvent> AttendingRequests { get; set; }

        public ElevatorViewModel(Elevator elevator)
        : base(elevator)
        {
            Location = elevator.Location;
            PeopleCount = elevator.AttendingRequests.Count;
            Id = elevator.Id;
            Direction = elevator.Direction;
            AttendingRequests = (List<IRequestEvent>)elevator.AttendingRequests;
        }

        object ICloneable.Clone() { return Clone(); }
        public ElevatorViewModel Clone()
        {
            return new ElevatorViewModel(Model)
            {
                Location = Location,
                PeopleCount = PeopleCount,
                Direction = Direction,
                AttendingRequests = new List<IRequestEvent>(AttendingRequests)
            };
        } 
    }
}
