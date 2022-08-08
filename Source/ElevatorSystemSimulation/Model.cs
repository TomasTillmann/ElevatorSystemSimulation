﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTypes;
using Interfaces;


namespace Model {
    public class Elevator : IReadOnlyElevator, IElevator {
        internal Building? Building { get; set; }
        internal Simulation.Simulation? Simulation { get; set; }


        public MetersPerSecond TravelSpeed { get; private set; }
        public MetersPerSecond AccelerationDelaySpeed { get; private set; }
        public TimeSpan DepartingTime { get; private set; }
        public int Capacity { get; private set; }
        public FloorLocation? MaxFloorLocation { get; private set; }
        public FloorLocation? MinFloorLocation { get; private set; }
        public DateTime WhenAvailable { get; set; }
        public ElevatorLocation Location { get; set; }

        public Elevator(ElevatorLocation location) {
            Location = location;
        }

        public void MoveUp() {
            // calculate WhenAvailable
            PlanThisElevator();
        }
        public void MoveDown() { 
            PlanThisElevator();
        }

        public void Idle() {
            PlanThisElevator();
        }

        public void Depart() {
            PlanThisElevator();
        }

        private void PlanThisElevator() {
            if(Simulation == null) {
                throw new InvalidOperationException("Internal Error - Simulation has to be set on each elevator");
            }
            Simulation.PlanElevator(this);
        }

        public override bool Equals(object? obj) {
            return obj is Elevator elevator &&
                   EqualityComparer<ElevatorSystem?>.Default.Equals(Building?.ElevatorSystem, elevator.Building?.ElevatorSystem) &&
                   EqualityComparer<MetersPerSecond>.Default.Equals(TravelSpeed, elevator.TravelSpeed) &&
                   EqualityComparer<MetersPerSecond>.Default.Equals(AccelerationDelaySpeed, elevator.AccelerationDelaySpeed) &&
                   DepartingTime.Equals(elevator.DepartingTime) &&
                   Capacity == elevator.Capacity;
        }

        public override int GetHashCode() {
            return HashCode.Combine(TravelSpeed, AccelerationDelaySpeed, DepartingTime, Capacity);
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


        public bool IsFreezed { get; set; }
        public List<Floor> Floors { get; set; } 
        public ElevatorSystem ElevatorSystem { get; set; }
        public Population Population { get; set; }

        public Building(bool isFreezed, List<Floor> floors, ElevatorSystem elevatorSystem, Population population) {
            IsFreezed = isFreezed;
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
        public List<Elevator> Elevators { get; set; }
        private Building _Building { get; set; }

        public ElevatorSystem(List<Elevator> elevators, Building building) {
            Elevators = elevators;
            _Building = building;

            foreach (var elevator in elevators) {
                elevator.Building = _Building;
            }
        }
    }
}