namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface IElevatorLogic
        {
            public void Execute(ISimulationState state);
        }

        public interface IEvent : ILocatable
        {
            Seconds WhenPlanned { get; }
            Floor EventLocation { get; }
        }
        public interface IRestartable
        {
            void Restart();
        }

        public interface IRequestEvent : IEvent
        {
            Floor Destination { get; }
        }

        public interface ILocatable
        {
            Centimeters Location { get; }
        }

        public interface ISimulationState
        {
            public IEvent CurrentEvent { get; }
            public Seconds CurrentTime { get; }
        }

        public interface ISimulationState<EventType> where EventType : IEvent
        {
            public EventType CurrentEvent { get; }
            public Seconds CurrentTime { get; }
        }
    }
}
