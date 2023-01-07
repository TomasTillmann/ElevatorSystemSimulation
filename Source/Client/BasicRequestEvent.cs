using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;

namespace Client
{
    public class BasicRequestEvent : RequestEvent
    {
        public BasicRequestEvent(Floor eventLocation, Seconds whenPlanned, Floor destination)
            :base(whenPlanned, eventLocation, destination) { }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Floor: {EventLocation.Location}\n" +
            $"Destination: {Destination}";
    }
}