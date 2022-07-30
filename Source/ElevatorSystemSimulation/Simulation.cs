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
        private Calendar _Calendar { get; set; }

        protected override Action<Event>? CurrentAlgorithm { get; set; } 

        public Statistics? Statistics { get; set; }
        public int RunCount { get; set; }

        private Building _Building;
        public override Building Building {
            get {
                return _Building;
            }
            set {
                _Building = value;
                _RequestGenerator = new RequestGenerator(Building);
            }
        }

        private TimeSpan? _DurationTime;
        public TimeSpan? DurationTime { 
            get { 
                return _DurationTime; 
            } 
            set {
                _DurationTime = value;
                _PeopleToDepart = null;
            } 
        }

        private int? _PeopleToDepart;
        public int? PeopleToDepart {
            get {
                return _PeopleToDepart;
            }
            set {
                _PeopleToDepart = value;
                _DurationTime = null;
            }
        }

        public Simulation(Building building) {
            Building = building;
            _Calendar = new();
        }


        public void SetAlgorithmsToCompare(List<Action> algorithms) {

        }

        public void SetRequestsToGenerate(List<Request> requests) {

        }

        public void Run() {
            if(Building == null) {
                throw new InvalidOperationException("Cannot run simulation on building. Building wasn't set. It is null.");
            }

            Building.IsFreezed = true;

            if(PeopleToDepart != null) {
                while(_DepartedPeopleCount < PeopleToDepart) {
                    _InternalRun();
                }
            }
            else if(DurationTime != null) {
                while(_Time < DurationTime) {
                    _InternalRun();
                }
            }
            
            else {
                throw new InvalidOperationException("Neither DurationTime or PeopleToDepart was set! At least one must be.");
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


            CurrentAlgorithm(_Calendar.GetEvent());
            ResetAfterStep();
        }

        private void ResetAfterStep() {
            if(Building == null) {
                throw new ArgumentNullException("Cannot run simulation on building. Building wasn't set. It is null.");
            }
        }
    }

    public class Calendar {
        private PriorityQueue<Event, DateTime> Events { get; set; } = new();
        public Event GetEvent() {
            return Events.Dequeue();
        }

        public void AddEvent(Event e) {
            Events.Enqueue(e, e.WhenAvailable);
        }
    }

    public class Event {
        public DateTime WhenAvailable { get; set; }

        private Elevator _Elevator;
        public Elevator Elevator {
            get {
                return _Elevator;
            }
            set {
                _Elevator = value;
                WhenAvailable = value.WhenFinished;
            }
        }

        public Event(Elevator elevator) {
            _Elevator = elevator;
        }
    }

    public abstract class AlgorithmEnvironment {
        public virtual object? Resources { get; set; }
        public abstract Building Building { get; set; }
        protected abstract Action<Event>? CurrentAlgorithm { get; set; }
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
