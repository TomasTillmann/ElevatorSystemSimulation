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

        // client implements his own request event - this request event defines capabilities of the elevator system
        public interface IRequestEvent : IEvent
        {
            Floor Destination { get; }
        }
    }
}
