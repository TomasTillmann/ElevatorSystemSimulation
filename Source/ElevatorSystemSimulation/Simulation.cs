using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;
using Interfaces;

namespace Simulation {
    public class Simulation : IPlanner {
        private int _Time { get; set; }
        private int _DepartedPeopleCount { get; set; }
        private Generator _Generator { get; set; }
        private Calendar _Calendar { get; set; } = new();
        private IElevatorLogic CurrentLogic { get; set; } 


        public Statistics Statistics { get; set; } = new();

        private Building _Building;
        public Building Building {
            get {
                return _Building;
            }
            set {
                _Building = value;
                _Generator = new Generator(Building);
            }
        }

        private int? _TotalTime;
        public int? TotalTime { 
            get { 
                return _TotalTime; 
            } 
            set {
                _TotalTime = value;
                _TotalPeopleToDepart = null;
            } 
        }

        private int? _TotalPeopleToDepart;
        public int? TotalPeopleToDepart {
            get {
                return _TotalPeopleToDepart;
            }
            set {
                _TotalPeopleToDepart = value;
                _TotalTime = null;
            }
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Simulation(Building building) {
            #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            Building = building;
        }

        public void SetElevatorLogicsToCompare(List<IElevatorLogic> elevatorLogics) {
            if(elevatorLogics == null || elevatorLogics.Count == 0) {
                throw new InvalidOperationException("There needs to be at least one algorithm provided.");
            }
        }

        public void SetRequestsToGenerate(List<IRequest> requests) {
            if(requests == null || requests.Count == 0) {
                throw new InvalidOperationException("There needs to be at least one request provided.");
            }
        }

        public void PlanElevator(IElevator elevator, int plannedTime) {
            IEvent e = new ElevatorAvailableEvent(elevator, plannedTime);
            _Calendar.AddEvent(e);
        }

        public void UnplanElevator(IElevator elevator) {

        }

        public void Run() {
            foreach(NewRequestEvent request in _Generator.GenerateNewRequestEvents()) {
                _Calendar.AddEvent(request);
            }

            if(CurrentLogic == null) {
                throw new ArgumentNullException("Cannot run simulation. There were no algorithms set!");
            }

            if(TotalPeopleToDepart != null) {
                while(_DepartedPeopleCount < TotalPeopleToDepart) {
                    _InternalRun();
                }
            }
            else if(TotalTime != null) {
                while(_Time < TotalTime) {
                    _InternalRun();
                }
            }
            else {
                throw new InvalidOperationException("Neither DurationTime or PeopleToDepart was set! At least one must be.");
            }
        }

        private void _InternalRun() {
            CurrentLogic.Step(_Calendar.GetEvent());
            _ResetAfterStep();
        }

        private void _ResetAfterStep() {

        }
    }

    internal class Calendar {
        private PriorityQueue<IEvent, int> Events { get; set; } = new();

        public Calendar() {
            //Events.Comparer = // from minimal to maximum TODO:
        }
        public IEvent GetEvent() {
            return Events.Dequeue();
        }

        public void AddEvent(IEvent e) {
            Events.Enqueue(e, e.PlannedTime);
        }

        public void Clear() {
            Events.Clear();
        }
    }

    public struct ElevatorAvailableEvent : IEvent {
        public int PlannedTime { get; }
        public IElevator Elevator { get; }

        public ElevatorAvailableEvent(IElevator elevator, int plannedTime) {
            Elevator = elevator;
            PlannedTime = plannedTime;
        }
    }

    public struct NewRequestEvent : IEvent {
        public int PlannedTime { get; }
        public IRequest Request { get; }

        public NewRequestEvent(IRequest request, int plannedTime) {
            PlannedTime = plannedTime;
            Request = request;
        }
    }

    public struct Request : IRequest {
        public FloorLocation FloorLocation { get; set; }
        public int ProbabilityToGenerate { get; set; }
        public int? NumberOfPersons { get; set; }
        public FloorLocation? ToFloorLocation { get; set; }
        public int? Priority { get; set; }
        public List<int>? AllowedElevators { get; set; }
    }

    internal class Generator {
        private Building _Building { get; set; }
        public Generator(Building building) {
            _Building = building;
        }

        public List<NewRequestEvent> GenerateNewRequestEvents() {
            return new();
        }
        public List<Request> GenerateRequests() {
            return new();
        }
    }
}
