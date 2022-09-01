using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using DataTypes;
using Interfaces;

namespace MainLogic {
    public class Simulation {
        private Calendar _Calendar { get; set; } = new();
        private Statistics _Statistics { get; set; } = new();

        public Seconds CurrentTime { get; private set; }
        public IElevatorLogic CurrentLogic { get; }  
        public Building Building { get; }
        public Seconds TotalTime { get; }
        public Generator Generator { get; }

        public Simulation(IElevatorLogic currentLogic, Building building, Seconds totalTime, Generator generator) {
            CurrentLogic = currentLogic;
            Building = building;
            TotalTime = totalTime;
            Generator = generator;
        }

        public void PlanElevator(IElevator elevator, Seconds duration, Centimeters location) {
            _Calendar.AddEvent(new ElevatorEvent(elevator, CurrentTime + duration, location));
        }

        private void UnplanElevator(IElevator elevator) {

        }

        public void Run() {
            foreach(NewRequestEvent request in Generator.GenerateNewRequestEvents()) {
                _Calendar.AddEvent(request);
            }

            while(CurrentTime < TotalTime) {
                Step();
            }
        }

        private void Step() {
            CurrentLogic.Step(_Calendar.GetEvent());
            _ResetAfterStep();
        }

        private void _ResetAfterStep() {

        }
    }

    public struct ElevatorEvent : IEvent {
        public Seconds PlannedTime { get; }
        public IElevator Elevator { get; }
        public Centimeters Location { get; }

        public ElevatorEvent(IElevator elevator, Seconds plannedTime, Centimeters location) {
            Elevator = elevator;
            PlannedTime = plannedTime;
            Location = location;
        }
    }

    public struct NewRequestEvent : IEvent {
        public Seconds PlannedTime { get; }
        public IRequest Request { get; }

        public NewRequestEvent(IRequest request, Seconds plannedTime) {
            PlannedTime = plannedTime;
            Request = request;
        }
    }

    public struct Request : IRequest {
        public Floor Floor { get; set; }
        public int ProbabilityToGenerate { get; set; }
        public int? NumberOfPersons { get; set; }
        public Floor? ToFloor { get; set; }
        public int? Priority { get; set; }
        public List<int>? AllowedElevators { get; set; }
    }

    internal class Calendar {
        private PriorityQueue<IEvent, Seconds> Events { get; }

        public Calendar() {
            Events = new PriorityQueue<IEvent, Seconds>(new TimeComparer());
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

        private class TimeComparer : Comparer<Seconds> {
            public override int Compare(Seconds x, Seconds y) {
                return x.Value.CompareTo(y.Value);
            }
        }
    }

    public class Generator {
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
