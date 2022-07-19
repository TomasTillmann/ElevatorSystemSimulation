using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;

namespace Simulation {
    public class Simulation : AlgorithmEnvironment {

        private TimeSpan _Time { get; set; }
        private int _DepartedPeopleCount { get; set; }
        private RequestGenerator? _RequestGenerator { get; set; }
        private Building? _Building;
        private TimeSpan? _DurationTime;
        private int? _PeopleToDepart;

        protected override Action<List<Request>>? CurrentAlgorithm { get; set; } 


        public override Building? Building {
            get {
                return _Building;
            }
            set {
                _Building = value;
                _RequestGenerator = new RequestGenerator(Building);
            }
        }
        public Statistics? Statistics { get; set; }
        public int RunCount { get; set; }
        public TimeSpan? DurationTime { 
            get { 
                return _DurationTime; 
            } 
            set {
                _DurationTime = value;
                _PeopleToDepart = null;
            } 
        }
        public int? PeopleToDepart {
            get {
                return _PeopleToDepart;
            }
            set {
                _PeopleToDepart = value;
                _DurationTime = null;
            }
        } 

        public Simulation() { }

        public Simulation(Building building) {
            Building = building;
            _RequestGenerator = new RequestGenerator(Building);
        }


        public void SetAlgorithmsToCompare(List<Action> algorithms) {

        }

        public void SetRequestsToGenerate(List<Request> requests) {

        }

        public void Run() {
            if(Building == null) {
                throw new ArgumentNullException("Cannot run simulation on building. Building wasn't set. It is null.");
            }

            Building.IsFreezed = true;

            if(DurationTime == null) {
                while(_DepartedPeopleCount < PeopleToDepart) {
                    _InternalRun();
                }
            }
            else {
                while(_Time < DurationTime) {
                    _InternalRun();
                }
            }

            Building.IsFreezed = false;
        }

        private void _InternalRun() {
            if(CurrentAlgorithm == null) {
                throw new ArgumentNullException("Cannot run simulation. There were no algorithms set!");
            }

            if(_RequestGenerator == null) {
                throw new InvalidOperationException("Internal Request generator isn't set, and it should be.");
            }


            CurrentAlgorithm(_RequestGenerator.GenerateRequests());
            ResetAfterStep();
        }

        private void ResetAfterStep() {
            if(Building == null) {
                throw new ArgumentNullException("Cannot run simulation on building. Building wasn't set. It is null.");
            }

            Building.ElevatorSystem.StepDuration = null;
        }
    }

    public abstract class AlgorithmEnvironment {
        public virtual object? Resources { get; set; }
        public abstract Building? Building { get; set; }
        protected abstract Action<List<Request>>? CurrentAlgorithm { get; set; }
    }

    public struct Request {
        public FloorLocation FloorLocation { get; set; }
        public int ProbabilityToGenerate { get; set; }
        public int? NumberOfPersons { get; set; }
        public FloorLocation? ToFloorLocation { get; set; }
        public int? Priority { get; set; }
        public List<int>? AllowedElevators { get; set; }
    }

    internal class RequestGenerator {
        private Building? _Building { get; set; }
        public RequestGenerator(Building? building) {
            _Building = building;
        }

        public List<Request> GenerateRequests() {
            throw new NotImplementedException();
        }
    }
}
