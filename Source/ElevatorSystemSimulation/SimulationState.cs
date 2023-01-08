using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public struct SimulationState : ISimulationState
    {
        public IEvent CurrentEvent { get; }
        public Seconds CurrentTime { get; } 

        public SimulationState(IEvent currentEvent, Seconds currentTime)
        {
            CurrentEvent = currentEvent;
            CurrentTime = currentTime;
        }
    }

    public struct SimulationState<TEventType> : ISimulationState<TEventType> where TEventType : IEvent
    {
        public TEventType Event { get; }
        public Seconds Time { get; }

        public SimulationState(TEventType currentEvent, Seconds currentTime)
        {
            Event = currentEvent;
            Time = currentTime;
        }
    }
}
