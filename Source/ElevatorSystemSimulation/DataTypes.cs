using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace DataTypes {
    public struct MetersPerSecond {
        public double Value { get; }

        public MetersPerSecond(double value) {
            Value = value;
        }

        public static Meters operator * (MetersPerSecond speed, TimeSpan time) {
            return new Meters(speed.Value * time.Seconds);
        }

        public static Meters operator * (TimeSpan time, MetersPerSecond speed) {
            return new Meters(speed.Value * time.Seconds);
        }
    }

    public struct FloorLocation {
        public int Floor { get; }

        public FloorLocation(int floor) {
            Floor = floor;
        }
    }

    public class ElevatorLocation {
        public int Floor { get; set; }
        public Meters MetersBetweenFloors { get; set; }

        public ElevatorLocation(int floor, Meters metersBetweenFloors) {
            Floor = floor;
            MetersBetweenFloors = metersBetweenFloors;
        }
    }

    public struct Meters {
        public double Value { get; }
        public Meters(double value) {
            Value = value;
        }

        public static Meters operator + (Meters m1, Meters m2) {
            return new Meters(m1.Value + m2.Value);
        }

        public static implicit operator Meters(int value) => new Meters(value);
    }

    public class PopulationDistribution {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        // TODO add actual distribution representation
    }

    public enum ElevatorAction {
        MoveUp,
        MoveDown,
        Idle,
        Depart,
    }
}
