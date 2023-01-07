using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;

namespace Client
{
    public struct BasicRequestEvent : IRequestEvent
    {
        #region Identification
        public int Id { get; } 
        private static int Counter = 0;
        #endregion

        public Seconds WhenPlanned { get; } 
        public Floor EventLocation { get; } 
        public Floor Destination { get; }

        public Centimeters Location => EventLocation.Location;

        public BasicRequestEvent(Floor eventLocation, Seconds whenPlanned, Floor destination)
        {
            EventLocation = eventLocation;
            WhenPlanned = whenPlanned;
            Destination = destination;

            Id = Counter;
            Counter++;
        }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Floor: {EventLocation.Location}\n" +
            $"Destination: {Destination}";
    }
}