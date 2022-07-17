using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;

namespace Simulation {
    public class Simulation : AlgorithmEnvironment {
        public Simulation(Building building) {
            Building = building;
        }

        public TimeSpan DurationTime { get; set; }
        public Statistics Statistics { get; set; }
        public int RunCount { get; set; }
        public int PeopleToDepart { get; set; }
        public void SetAlgorithmsToCompare(List<Action> algorithms) {

        }
        public void SetRequestsToGenerate(List<Request> requests) {

        }

        public void Run() {

        }
    }

    public abstract class AlgorithmEnvironment {
        protected Building Building { get; set; }
        protected List<Request> Requests { get; set; }
        protected Action Algorithm { get; set; }
    }

    public struct Request {
        public FloorLocation FloorLocation { get; set; }
        public int? NumberOfPersons { get; set; }
        public FloorLocation? ToFloorLocation { get; set; }
        public int? Priority { get; set; }
        public List<int>? AllowedElevators { get; set; }
    }

    //public class RequestType {
    //    public List<(FloorLocation FloorLocation, int Probability)> FloorLocationsAllowed { get; set; }
    //    public List<(int NumberOfPersons, int Probability)> NumbersOfPersonsAllowed { get; set; }
    //    public List<(FloorLocation FloorLocation, int Probability)> ToFloorLocationsAllowed { get; set; }
    //    public List<(int Priority, int Probability)> PrioritiesAllowed { get; set; }
    //    public List<(List<int> AllowedElevators, int Probability)> AllowedElevatorsAllowed { get; set; }
    //}
}
