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
    }

    public struct FloorLocation {
        public double Id { get; }
        public double Height { get; }

        public FloorLocation(double id, double height) {
            Id = id;
            Height = height;
        }
    }

    public struct Meters {
        public double Value { get; }
        public Meters(double value) {
            Value = value;
        }
    }

    public class PopulationDistribution {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        // TODO add actual distribution representation
    }
}
