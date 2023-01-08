namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface ISimulationState
        {
            public IEvent CurrentEvent { get; }
            public Seconds CurrentTime { get; }
        }
        public interface ISimulationState<TEvent> where TEvent : IEvent
        {
            public TEvent Event { get; }
            public Seconds Time { get; }
        }
    }
}
