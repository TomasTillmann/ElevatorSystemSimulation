using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;
using Interfaces;

namespace Simulation {

    public interface AlgorithmEnvironment {
        public object? Resources { get; set; }
        public Building Building { get; set; }
    }

    public class Simulation : AlgorithmEnvironment {
        private TimeSpan _Time { get; set; }
        private int _DepartedPeopleCount { get; set; }
        private RequestGenerator _RequestGenerator { get; set; }
        private Calendar _Calendar { get; set; }
        private Action<Event>? CurrentAlgorithm { get; set; } 


        public object? Resources { get; set; }
        public Statistics? Statistics { get; set; }
        public int RunCount { get; set; }

        private Building _Building;
        public Building Building {
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

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Simulation(Building building) {
            #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            Building = building;
            _Calendar = new();
        }


        public void SetAlgorithmsToCompare(List<Action> algorithms) {
            if(algorithms == null || algorithms.Count == 0) {
                throw new InvalidOperationException("There needs to be at least one algorithm provided.");
            }
        }

        public void SetRequestsToGenerate(List<Request> requests) {
            if(requests == null || requests.Count == 0) {
                throw new InvalidOperationException("There needs to be at least one request provided.");
            }
        }

        public void Run() {
            Building.IsFreezed = true;
            //TODO: feed calendar with some initial events

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

        internal void PlanElevator(Elevator elevator) {
            Event e = new(elevator);
            _Calendar.AddEvent(e);
        }

        private void _InternalRun() {
            if(CurrentAlgorithm == null) {
                throw new ArgumentNullException("Cannot run simulation. There were no algorithms set!");
            }

            Event e = _Calendar.GetEvent();
            e.NewRequests = _RequestGenerator.GenerateRequests();
            CurrentAlgorithm(e);
            _ResetAfterStep();
        }

        private void _ResetAfterStep() {
            if(Building == null) {
                throw new ArgumentNullException("Cannot run simulation on building. Building wasn't set. It is null.");
            }
        }
    }

    internal class Calendar {
        private PriorityQueue<Event, DateTime> Events { get; set; } = new();

        public Calendar() {
            //Events.Comparer = // from minimal to maximum TODO:
        }
        public Event GetEvent() {
            return Events.Dequeue();
        }

        public void AddEvent(Event e) {
            Events.Enqueue(e, e.WhenAvailable);
        }

        public void Clear() {
            Events.Clear();
        }
    }

    public class Event {
        public DateTime WhenAvailable { get; set; }
        public List<Request> NewRequests { get; set; } = new();

        private IReadOnlyElevator _Elevator;
        public IReadOnlyElevator Elevator {
            get {
                return _Elevator;
            }
            set {
                _Elevator = value;
                WhenAvailable = value.WhenAvailable;
            }
        }

        public Event(IReadOnlyElevator elevator) {
            _Elevator = elevator;
        }
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
            return new();
        }
    }
}
