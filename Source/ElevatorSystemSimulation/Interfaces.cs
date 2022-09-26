namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface IElevatorLogic
        {
            public void Step(IEvent e, Seconds currentTime);
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
    }
}
