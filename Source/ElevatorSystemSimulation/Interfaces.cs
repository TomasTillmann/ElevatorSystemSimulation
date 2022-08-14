using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;

namespace Interfaces {
    public interface IElevatorLogic {
        public IElevatorsStatusProvider _ElevatorsStatusProvider { get; set; }
        public Building Building { get; set; }
        public void Step(IEvent e);
    }

    public interface IEvent {
        public int PlannedTime { get; }
    }

    public interface IRequest {

    }

    public interface IElevatorsStatusProvider {

    }

    public interface IPlanner {
        public void PlanElevator(IElevator elevator, int plannedTime);
        public void UnplanElevator(IElevator elevator);
    }

    public interface IElevatorActions {
        public void MoveTo(int floor);
        public void Idle();
        public void Load();
    }

    public interface IElevator : IElevatorActions {
        public MetersPerSecond TravelSpeed { get; }
        public MetersPerSecond AccelerationDelaySpeed { get; }
        public TimeSpan DepartingTime { get; }
        public int Capacity { get; }
        public FloorLocation? MaxFloorLocation { get; }
        public FloorLocation? MinFloorLocation { get; }
        public ElevatorLocation Location { get; }
    }
}
