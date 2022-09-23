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

        // client implements his own request event - this request event defines capabilities of the elevator system
        public interface IRequestEvent : IEvent
        {
            Floor Destination { get; }
        }

        //public interface IElevatorActions
        //{
        //    bool IsIdle { get; set; }
        //    void MoveTo(Floor? floor);
        //    void Idle(Floor? floor);
        //    void Load(Floor? floor);
        //}

        //public interface IPlannableElevator
        //{
        //    Action<Elevator, Seconds, Floor>? PlanElevator { get; set; }
        //    Action<Elevator>? UnplanElevator { get; set; }
        //}

        //public interface IElevatorView : IElevatorActions
        //{
        //    int Id { get; }
        //    CentimetersPerSecond TravelSpeed { get; }
        //    CentimetersPerSecond AccelerationDelaySpeed { get; }
        //    Seconds DepartingTime { get; }
        //    int Capacity { get; }
        //}

        //public interface IPlannableActionableElevator : IPlannableElevator, IElevatorView
        //{
        //    Direction Direction { get; }
        //    Centimeters Location { get; set; }
        //    Seconds LastPlanned { get; set; }
        //}
    }
}
