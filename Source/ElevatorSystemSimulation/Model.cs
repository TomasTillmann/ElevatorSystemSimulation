using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTypes;

namespace Model {
    public class Elevator {
        public MetersPerSecond TravelSpeed { get; set; }
        public MetersPerSecond AccelerationDelaySpeed { get; set; }
        public TimeSpan DepartTime { get; set; }
        public int Capacity { get; set; }
        public FloorLocation FloorLocation { get; set; }
        public FloorLocation MaxFloorLocation { get; set; }
        public FloorLocation MinFloorLocation { get; set; }

        public void MoveUp() {

        }

        public void MoveDown() { 

        }

        public void Idle() {

        }

        public void DepartIn() {

        }

        public void DeaprtOut() {

        }
    }

    public class Floor {
        public string FloorName { get; set; }
        public FloorLocation Location { get; set; }
        public Meters Height { get; set; }
    }

    public class Population {
        public PopulationDistribution Distribution { get; set; }
        public List<(PopulationDistribution, TimeSpan ElapsedTime)> DistributionInTime { get; set; }
    }

    public class Building {
        public List<Floor> Floors { get; set; }
        public List<Elevator> Elevators { get; set; }
        public List<Population> Population { get; set; }
    }

    public class Statistics {

    }
}