namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface IElevatorLogic
        {
            Building Building { get; }
            void Execute(ISimulationState state);
        }

        public interface IElevatorLogic<TRequestEvent> : IElevatorLogic where TRequestEvent : Request
        {
            void Execute(ISimulationState<TRequestEvent> requestEvent);
            void Execute(ISimulationState<ElevatorEvent> elevatorEvent);
        }

        public interface IRestartable
        {
            void Restart();
        }

        public interface ISimulation : IRestartable
        {
            Building Building { get; }
            Seconds CurrentTime { get; }
            IEvent? LastEvent { get; }
            IEvent? LastAction { get; }
            bool IsOver { get; }
            int StepCount { get; }

            void Run();
            void Step();
        }

        public interface IIdentifiable
        {
            int Id { get; }
        }

        public interface IEvent
        {
            Seconds WhenPlanned { get; }
            Floor EventLocation { get; }
        }

        /// <summary>
        /// Client request custom type has to inherit from this RequestEvent class.
        /// </summary>
        /// <param name="whenPlanned">
        /// When this event will happen in the simulation.
        /// </param>
        /// <param name="eventLocation">
        /// In what floor this request takes place.
        /// </param>
        /// <param name="destination">
        /// In what floor the person behind this request wants to travel to.
        /// </param>
        public abstract class Request : IIdentifiable, ILocatable, IEvent
        {
            #region Identification

            public int Id { get; } 
            private static int Counter = 0;

            #endregion

            public Seconds WhenPlanned { get; } 
            public Floor EventLocation { get; } 
            public Centimeters Location => EventLocation.Location;
            public Floor Destination { get; set; }

            internal RequestInfo? Info { get; set; }

            protected Request(Seconds whenPlanned, Floor eventLocation, Floor destination)
            {
                WhenPlanned = whenPlanned;
                EventLocation = eventLocation;
                Destination = destination;
            }
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

        public interface ISimulationState<TEvent> where TEvent : IEvent
        {
            public TEvent Event { get; }
            public Seconds Time { get; }
        }
    }
}
