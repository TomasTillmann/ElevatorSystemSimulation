using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;

namespace Client
{
    public struct BasicRequestEvent : IRequestEvent
    {
        public Seconds WhenPlanned { get; } 
        public Floor EventLocation { get; } 
        public Floor Destination { get; }

        public BasicRequestEvent(Floor eventLocation, Seconds whenPlanned, Floor destination)
        {
            EventLocation = eventLocation;
            WhenPlanned = whenPlanned;
            Destination = destination;
        }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Floor: {EventLocation.Location}\n" +
            $"Destination: {Destination}";
    }
}