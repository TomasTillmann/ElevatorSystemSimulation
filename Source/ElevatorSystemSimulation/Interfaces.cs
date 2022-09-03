using DataTypes;
using Model;

namespace Interfaces
{
    public interface IElevatorLogic
    {
        IBuilding Building { get; set; }
        void Step(IEvent e);
    }

    public interface IElevatorSystem : IElevatorsStatusProvider
    {
        List<IElevator> Elevators { get; set; }
    }

    public interface IBuilding
    {
        IElevatorSystem ElevatorSystem { get; }
        IFloors Floors { get; }
    }

    public interface IFloors
    {
        List<IFloor> Value { get; }
    }

    public interface IFloor
    {
        Centimeters Location { get; set; }
        int FloorId { get; }
        Centimeters Height { get; }
        string? Name { get; }
    }

    public interface IPopulation
    {

    }

    public interface IEvent
    {
        Seconds WhenPlanned { get; }
    }

    public interface IRequest 
    {
        Seconds WhenPlanned { get; }
        //TODO other stuff
    }

    public interface IElevatorsStatusProvider
    {

    }

    public interface IElevatorActions
    {
        void MoveTo(IFloor? floor);
        void Idle();
        void Load();
    }

    public interface IPlannableElevator
    {
        Action<Elevator, Seconds, Centimeters>? PlanElevator { get; set; }
        Action<Elevator>? UnplanElevator { get; set; }
        public bool IsPlanned { get; set; }
    }

    public interface IElevator : IElevatorActions
    {
    }

    public interface IPlannableActionableElevator : IPlannableElevator, IElevator
    {
    }
}
