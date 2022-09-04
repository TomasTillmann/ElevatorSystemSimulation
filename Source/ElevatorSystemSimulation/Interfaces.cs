namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface IElevatorLogic
        {
            Building Building { get; set; }
            void Step(IEvent e);
        }

        public interface IEvent
        {
            Seconds WhenPlanned { get; }
        }

        // client implements his own request event - this request event defines capabilities of the elevator system
        public interface IRequestEvent : IEvent
        {
            Floor Floor { get; }
        }

        public interface IElevatorActions
        {
            bool IsAvailable { get; set; }
            void MoveTo(Floor? floor);
            void Idle(Floor? floor);
            void Load(Floor? floor);
        }

        public interface IPlannableElevator
        {
            Action<Elevator, Seconds, Floor>? PlanElevator { get; set; }
            Action<Elevator>? UnplanElevator { get; set; }
        }

        public interface IElevatorView : IElevatorActions
        {
            //TODO - other stuff - like some elevator's parameters (speed, capacity, ... )
        }

        public interface IPlannableActionableElevator : IPlannableElevator, IElevatorView
        {
        }
    }
}
