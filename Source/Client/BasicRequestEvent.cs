using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;

namespace Client
{
    public struct BasicRequestEvent : IRequestEvent
    {
        public Seconds WhenPlanned { get; } 
        public Floor Floor { get; } 
        public Floor Destination { get; }

        public BasicRequestEvent(Floor floor, Seconds whenPlanned, Floor destination)
        {
            Floor = floor;
            WhenPlanned = whenPlanned;
            Destination = destination;
        }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Floor: {Floor.Location}\n" +
            $"Destination: {Destination}";
    }
}