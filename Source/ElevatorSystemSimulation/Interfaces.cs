using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;

namespace Interfaces {
    public interface IElevatorLogic {
        IBuilding Building { get; set; }
        void Step(IEvent e);
    }

    public interface IElevatorSystem : IElevatorsStatusProvider {
        List<IElevator> Elevators { get; set; }
    }

    public interface IFloors {
        List<IFloor> Floors { get; set; }

    }
    
    public interface IBuilding {
    }

    public interface IFloor {
        int FloorId { get; }
        Centimeters Location { get; }
        Centimeters Height { get; }
        string? Name { get; }
    }

    public interface IPopulation {

    }

    public interface IEvent {
        Seconds PlannedTime { get; }
    }

    public interface IRequest {

    }

    public interface IElevatorsStatusProvider {

    }

    public interface IElevatorActions {
        void MoveTo(Floor floor);
        void Idle();
        void Load();
    }

    public interface IElevator : IElevatorActions {
    }
}
