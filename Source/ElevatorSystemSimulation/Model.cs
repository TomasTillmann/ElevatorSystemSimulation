using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTypes;


namespace Model {
    public class Elevator {
        private ElevatorSystem? ElevatorSystem { get; set; }


        public MetersPerSecond TravelSpeed { get; set; }
        public MetersPerSecond AccelerationDelaySpeed { get; set; }
        public TimeSpan DepartingTime { get; set; }
        public int Capacity { get; set; }
        public FloorLocation Location { get; set; }
        public FloorLocation? MaxFloorLocation { get; set; }
        public FloorLocation? MinFloorLocation { get; set; }

        public void MoveUp() {
            ElevatorSystem?.FindStepDuration();


        }

        public void MoveDown() { 
            ElevatorSystem?.FindStepDuration();
        }

        public void Idle() {
            ElevatorSystem?.FindStepDuration();

        }

        public void DepartIn() {
            ElevatorSystem?.FindStepDuration();

        }

        public void DepartOut() {
            ElevatorSystem?.FindStepDuration();

        }

        public override bool Equals(object? obj) {
            return obj is Elevator elevator &&
                   EqualityComparer<ElevatorSystem?>.Default.Equals(ElevatorSystem, elevator.ElevatorSystem) &&
                   EqualityComparer<MetersPerSecond>.Default.Equals(TravelSpeed, elevator.TravelSpeed) &&
                   EqualityComparer<MetersPerSecond>.Default.Equals(AccelerationDelaySpeed, elevator.AccelerationDelaySpeed) &&
                   DepartingTime.Equals(elevator.DepartingTime) &&
                   Capacity == elevator.Capacity;
        }

        public override int GetHashCode() {
            return HashCode.Combine(ElevatorSystem, TravelSpeed, AccelerationDelaySpeed, DepartingTime, Capacity);
        }
    }

    public class Floor {
        public string? FloorName { get; set; }
        public FloorLocation Location { get; set; }
        public Meters Height { get; set; }
    }

    public class Population {
        public List<PopulationDistribution> Distribution { get; set; }


        public Population(List<PopulationDistribution> distribution) {
            Distribution = distribution;
        }
    }

    public class Building {
        public bool IsFreezed { get; set; }
        public List<Floor> Floors { get; set; }
        public ElevatorSystem ElevatorSystem { get; set; }
        public List<Population> Population { get; set; }

        public Building(bool isFreezed, List<Floor> floors, ElevatorSystem elevatorSystem, List<Population> population) {
            IsFreezed = isFreezed;
            Floors = floors;
            ElevatorSystem = elevatorSystem;
            Population = population;
        }
    }

    public class Statistics {

    }

    public class ElevatorSystem {
        public List<Elevator> Elevators { get; set; }
        public TimeSpan? StepDuration { get; set; }

        public ElevatorSystem(List<Elevator> elevators, TimeSpan? stepDuration) {
            Elevators = elevators;
            StepDuration = stepDuration;
        }


        public void FindStepDuration() {
            if(StepDuration == null) {
                // calculation
            }

            return;
        }
    }
}