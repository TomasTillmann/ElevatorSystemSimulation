using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTypes;
using Interfaces;


namespace Model {
    public class Elevator : IElevator {
        private IPlanner _Planner { get; set; }

        public MetersPerSecond TravelSpeed { get; private set; }
        public MetersPerSecond AccelerationDelaySpeed { get; private set; }
        public TimeSpan DepartingTime { get; private set; }
        public int Capacity { get; private set; }
        public FloorLocation? MaxFloorLocation { get; private set; }
        public FloorLocation? MinFloorLocation { get; private set; }
        public ElevatorLocation Location { get; set; }

        public Elevator(IPlanner planner,
            MetersPerSecond travelSpeed,
            MetersPerSecond accelerationDelaySpeed,
            TimeSpan departingTime,
            int capacity,
            FloorLocation? maxFloorLocation,
            FloorLocation? minFloorLocation,
            ElevatorLocation location) {

            _Planner = planner;
            TravelSpeed = travelSpeed;
            AccelerationDelaySpeed = accelerationDelaySpeed;
            DepartingTime = departingTime;
            Capacity = capacity;
            MaxFloorLocation = maxFloorLocation;
            MinFloorLocation = minFloorLocation;
            Location = location;
        }

        public void MoveTo(int floor) {
            // here Planner plan elevator is called
        }

        public void Idle() {
        }

        public void Load() {
            // here Planner plan elevator is called
        }
    }

    public class Floor {
        public string? FloorName { get; set; }
        public FloorLocation Location { get; set; }
        public Meters Height { get; set; }
    }

    public class Population {
        public List<PopulationDistribution> Distribution { get; set; }
        public int TotalPeopleCount { get; set; }
        public int AveragePeopleCount { get; set; }


        public Population(List<PopulationDistribution> distribution) {
            Distribution = distribution;
        }
    }

    public class Building {
        private Dictionary<int, Floor> _Floors { get; set; } = new();


        public List<Floor> Floors { get; set; } 
        public ElevatorSystem ElevatorSystem { get; set; }
        public Population Population { get; set; }

        public Building(bool isFreezed, List<Floor> floors, ElevatorSystem elevatorSystem, Population population) {
            ElevatorSystem = elevatorSystem;
            Population = population;
            Floors = floors;

            foreach(var floor in floors) {
                _Floors.Add((int)floor.Location.Floor, floor);
            }
        }

        public Floor? GetFloor(ElevatorLocation Location) {
            if (_Floors.ContainsKey(Location.Floor)) {
                return _Floors[Location.Floor];
            }

            return null;
        }

        public Floor? GetUpperFloor(ElevatorLocation location) {
            if (_Floors.ContainsKey(location.Floor + 1)) {
                return _Floors[location.Floor + 1];
            }

            return null;
        }

        public Floor? GetLowerFloor(ElevatorLocation location) {
            if (_Floors.ContainsKey(location.Floor - 1)) {
                return _Floors[location.Floor - 1];
            }

            return null;
        }
    }

    public class Statistics {
        public int AverageWaitingTime { get; set; }
        public int LongestWaitingTime { get; set; }
        public int PeopleDepartedCount { get; set; }
        public int AverageElevatorTravelInFloors { get; set; }
        private List<int> _AllWaitingTimes = new();
        public List<int> AllWaitingTimes {
            get {
                _AllWaitingTimes.Sort();
                return _AllWaitingTimes;
            }
        } 
    }

    public class ElevatorSystem {
        public List<IElevator> Elevators { get; set; }

        public ElevatorSystem(List<IElevator> elevators) {
            Elevators = elevators;
        }
    }
}