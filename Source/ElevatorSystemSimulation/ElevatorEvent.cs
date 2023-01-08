using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public class ElevatorEvent : IEvent
    {
        public Seconds WhenPlanned { get; }
        public Elevator Elevator { get; }
        public Floor EventLocation { get; }
        public ElevatorAction FinishedAction { get; }
        public List<Request> DepartedRequests { get; }
        public Centimeters Location => EventLocation.Location;

        public ElevatorEvent(Elevator elevator, Seconds whenPlanned, Floor eventLocation, ElevatorAction finishedAction, List<Request> departedRequests)
        {
            Elevator = elevator;
            WhenPlanned = whenPlanned;
            EventLocation = eventLocation;
            FinishedAction = finishedAction;
            DepartedRequests = departedRequests;
        }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Elevator: {Elevator}\n" +
            $"Destination: {EventLocation}";
    }
}
