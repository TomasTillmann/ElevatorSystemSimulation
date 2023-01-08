namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
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
    }
}
