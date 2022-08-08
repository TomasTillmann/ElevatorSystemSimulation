using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;

namespace Interfaces {
    public interface IElevatorActions {
        public void MoveUp();
        public void MoveDown();
        public void Idle();
        public void Depart();
    }

    public interface IReadOnlyElevator : IElevatorActions {
        public MetersPerSecond TravelSpeed { get; }
        public MetersPerSecond AccelerationDelaySpeed { get; }
        public TimeSpan DepartingTime { get; }
        public int Capacity { get; }
        public FloorLocation? MaxFloorLocation { get; }
        public FloorLocation? MinFloorLocation { get; }
        public DateTime WhenAvailable { get; }
        public ElevatorLocation Location { get; }
    }

    public interface IElevator : IElevatorActions {
        public MetersPerSecond TravelSpeed { get; }
        public MetersPerSecond AccelerationDelaySpeed { get; }
        public TimeSpan DepartingTime { get; }
        public int Capacity { get; }
        public FloorLocation? MaxFloorLocation { get; }
        public FloorLocation? MinFloorLocation { get; }
        public DateTime WhenAvailable { get; set; }
        public ElevatorLocation Location { get; set; }
    }
}
