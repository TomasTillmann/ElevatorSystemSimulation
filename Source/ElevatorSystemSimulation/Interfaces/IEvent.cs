namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface IEvent
        {
            Seconds WhenPlanned { get; }
            Floor EventLocation { get; }
        }
    }
}
