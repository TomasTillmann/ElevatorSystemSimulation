namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface IElevatorLogic
        {
            public void Step(IEvent e);
        }

        public interface IEvent
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
    }
}
