using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTypes;
using Interfaces;
using MainLogic;
using Extensions;

namespace Model {
    public class Elevator : IElevator {
        private Simulation _Simulation { get; }


        public CentimetersPerSecond TravelSpeed { get; }
        public CentimetersPerSecond AccelerationDelaySpeed { get; }
        public Seconds DepartingTime { get; }
        public int Capacity { get; }
        public Floor? MaxFloorLocation { get; }
        public Floor? MinFloorLocation { get; }
        public Centimeters Location { get; set; }
        public int Direction { get; set; }


        public Elevator(
            CentimetersPerSecond travelSpeed,
            CentimetersPerSecond acceleratingTravelSpeed,
            Seconds departingTime,
            int capacity,
            Floor? maxFloorLocation,
            Floor? minFloorLocation,
            Centimeters location,
            Simulation simulation) {

            TravelSpeed = travelSpeed;
            AccelerationDelaySpeed = acceleratingTravelSpeed;
            DepartingTime = departingTime;
            Capacity = capacity;
            MaxFloorLocation = maxFloorLocation;
            MinFloorLocation = minFloorLocation;
            Location = location;
            _Simulation = simulation;
        }

        public void MoveTo(Floor floor) {
            Direction = GetDirection(floor.Location);
            _Simulation.PlanElevator(this, GetDistance(floor.Location) / TravelSpeed, floor.Location);
        }

        public void Idle() {
            _Simulation.PlanElevator(this, 0.ToSeconds(), Location);
        }

        public void Load() {
            _Simulation.PlanElevator(this, DepartingTime, Location);
        }

        private Centimeters GetDistance(Centimeters floorLocation) {
            Centimeters distance = floorLocation - Location;
            return distance.Value > 0 ?
                distance :
                new(-1 * distance.Value);
        }

        private int GetDirection(Centimeters floorLocation) {
            return (floorLocation - Location).Value > 0 ?
                1 :
                -1;
        }
    }

    public class Floor : IFloor {
        public int FloorId { get; }
        // Location represents ground of the floor, not ceiling
        public Centimeters Location { get; }
        public Centimeters Height { get; }
        public string? Name { get; }

        public Floor(int floorId, Centimeters location, Centimeters height, string? name = null) {
            FloorId = floorId;
            Location = location;
            Height = height;
            Name = name;
        }
    }

    public class Population : IPopulation {
        public List<PopulationDistribution> Distribution { get; set; }
        public int TotalPeopleCount { get; set; }
        public int AveragePeopleCount { get; set; }


        public Population(List<PopulationDistribution> distribution) {
            Distribution = distribution;
        }
    }

    public class Building : IBuilding {
        public List<IFloor> Floors { get; set; } 
        public IElevatorSystem ElevatorSystem { get; set; }
        public IPopulation Population { get; set; }

        public Building(bool isFreezed, List<IFloor> floors, ElevatorSystem elevatorSystem, Population population) {
            ElevatorSystem = elevatorSystem;
            Population = population;
            Floors = floors;

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

    public class ElevatorSystem : IElevatorSystem {
        public List<IElevator> Elevators { get; set; } = new();

        public ElevatorSystem() { }

        public ElevatorSystem(List<IElevator> elevators) {
            Elevators = elevators;
        }
    }
}