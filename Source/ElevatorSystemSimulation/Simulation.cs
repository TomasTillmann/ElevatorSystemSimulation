﻿using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public sealed class Simulation<TRequest> : IRestartable, ISimulation where TRequest : Request
    {
        private Calendar _Calendar { get; set; } = new();
        private Seconds _LastStepTime = 0.ToSeconds();
        private bool _DidClientMadeAction;
        private Statistics<TRequest> Statistics = new();

        public List<TRequest> AllRequests;
        public List<TRequest> DepartedRequests = new();

        public Building Building { get => _Building; set { _Building = value; Restart(); } }
        private Building _Building;

        public Seconds CurrentTime { get; private set; } = 0.ToSeconds();
        public IElevatorLogic<TRequest> CurrentLogic { get; set; }
        public int StepCount { get; private set; }
        public IEvent? LastEvent { get; private set; }
        public IEvent? LastAction { get; private set; }
        public bool IsOver { get; private set; }

        private ElevatorSystem ElevatorSystem => _Building.ElevatorSystem;
        private Floors Floors => _Building.Floors;

        public Simulation(
            Building building,
            IElevatorLogic<TRequest> currentLogic,
            List<TRequest> requests)
        {
            CurrentLogic = currentLogic;
            AllRequests = requests;
            _Building = building;

            _Calendar.Init(AllRequests);

            SetElevatorsIPlannableProperties();

            if(requests.All(r => r.WhenPlanned == 0.ToSeconds()))
            {
                IsOver = true;
            }
        }

        public void Run()
        {
            while (!IsOver)
            {
                Step();
            }
        }

        public void Step()
        {
            if (IsOver)
            {
                return;
            }

            if(_Calendar.TryGetEvent(out IEvent? e))
            {
                UpdateStateBeforeStep(e!);

                SimulationState state = new SimulationState(e!, CurrentTime);
                Execute(state);
                UpdateStats(state);

                UpdateStateAfterStep();
            }
            else
            {
                IsOver = true;
            }
        }

        public StatisticsResult GetStatistics()
        {
            return Statistics.GetResult(AllRequests.Select(r => (Request)r).ToList(), ElevatorSystem.Elevators);
        }

        public void Restart()
        {
            Building.ElevatorSystem.Elevators.ForEach(elevator => elevator.Restart());
            Building.Floors.Value.ForEach(floor => floor.Restart());

            // restart state
            IsOver = false;
            CurrentTime = 0.ToSeconds();
            _LastStepTime = 0.ToSeconds();
            StepCount = 0;
            LastEvent = null;
            LastAction = null;
            //

            _Calendar.Clear();
            _Calendar.Init(AllRequests);
        }

        //TODO: ugly
        private void Execute(ISimulationState state)
        {
            if (state.CurrentEvent is TRequest ce)
            {
                ce.EventLocation._Requests.Add(ce);

                CurrentLogic.Execute(new SimulationState<TRequest>(ce, state.CurrentTime));
            }
            else if (state.CurrentEvent is ElevatorEvent ee)
            {
                CurrentLogic.Execute(new SimulationState<ElevatorEvent>(ee, state.CurrentTime));
            }
            else
            {
                throw new ApplicationException("Event is something different. And it shouldn't be.");
            }
        }

        //TODO: ugly
        private void UpdateStats(ISimulationState state)
        {
            if (state.CurrentEvent is TRequest ce)
            {
                ce.EventLocation._Requests.Add(ce);

                Statistics.Update(new SimulationState<TRequest>(ce, state.CurrentTime));
            }
            else if (state.CurrentEvent is ElevatorEvent ee)
            {
                Statistics.Update(new SimulationState<ElevatorEvent>(ee, state.CurrentTime));
            }
            else
            {
                throw new ApplicationException("Event is something different. And it shouldn't be.");
            }
        }

        private void SetCurrentTime(Seconds whenPlanned)
        {
            _LastStepTime = CurrentTime;
            CurrentTime = whenPlanned;
        }

        private void SetElevatorsLocations(IEvent e)
        {
            foreach(Elevator elevator in Building.ElevatorSystem.Elevators)
            {
                elevator.SetLocation(CurrentTime - _LastStepTime);
            }

            // This is necessary, because everything is rounded to seconds, hence the location might be a little bit off (this is the only affected place by rounding) - FIX? measure in milliseconds
            if(e is ElevatorEvent ee)
            {
                ee.Elevator.Location = ee.EventLocation.Location;
            }

            ElevatorSystem.ValidateElevatorsLocations(Floors);
        }

        private void SetElevatorsIPlannableProperties()
        {
            foreach (Elevator elevator in Building.ElevatorSystem.Elevators)
            {
                elevator.PlanElevator = PlanElevator;
                elevator.UnplanElevator = UnplanElevator;
            }
        }

        private void PlanElevator(Elevator elevator, Seconds duration, Floor destination, ElevatorAction action)
        {
            _DidClientMadeAction = true;
            List<Request> currentlyDepartedRequests = new();

            if(action == ElevatorAction.MoveTo)
            {
                //TODO: also had to remove elevators from planned elevators
                destination._PlannedElevators.Add(elevator);
            }
            else if(action == ElevatorAction.Unload || action == ElevatorAction.UnloadAndLoad)
            {
                foreach(TRequest req in elevator.AttendingRequests.Where(r => r.Destination == destination))
                {
                    DepartedRequests.Add(req);
                    currentlyDepartedRequests.Add(req);
                }
            }

            ElevatorEvent ee = new ElevatorEvent(elevator, CurrentTime + duration, destination, action, currentlyDepartedRequests);

            LastAction = ee;

            _Calendar.AddEvent(ee);
        }

        private void UnplanElevator(Elevator elevator)
        {
            _Calendar.RemoveElevatorEvent(elevator);
        }

        private void UpdateStateBeforeStep(IEvent e)
        {
            SetCurrentTime(e.WhenPlanned);
            SetElevatorsLocations(e);

            //TODO: I think this is not correct, but somehow it works fine
            if (e is ElevatorEvent ee)
            {
                if (ee.FinishedAction == ElevatorAction.UnloadAndLoad)
                {
                    ee.EventLocation._PlannedElevators.Remove(ee.Elevator);
                }
            }
            //

            StepCount += 1;
            LastEvent = e;
        }

        private void UpdateStateAfterStep()
        {
            LastAction = _DidClientMadeAction ? LastAction : null;
            _DidClientMadeAction = false;
        }

        #region Calendar

        //TODO - implement better - its terribly slow like this - at worst O(n) is possible via Linked list
        // Priority Queue is not sufficient, does not allow for removing specific events (add another array or smthng needs to be done for it to work)
        private class Calendar
        {
            private readonly List<(IEvent Event, Seconds WhenPlanned)> _Events = new();

            public Calendar() { }

            public Calendar(IEnumerable<Request> requests)
            {
                Init(requests);
            }

            public bool TryGetEvent(out IEvent? e)
            {
                if(_Events.Count == 0)
                {
                    e = null;
                    return false;
                }

                var entry = _Events.OrderBy((entry) => entry.WhenPlanned.Value).First();
                _Events.Remove(entry);
                e = entry.Event;

                return true; 
            }

            public void AddEvent(IEvent e)
            {
                _Events.Add((e, e.WhenPlanned));
            }

            public void RemoveElevatorEvent(Elevator elevator)
            {
                for(int i = 0; i < _Events.Count; i++)
                {
                    var entry = _Events[i];
                    if(entry.Event is ElevatorEvent ee && ee.Elevator.Id == elevator.Id)
                    {
                        _Events.RemoveAt(i);
                        break;
                    }
                }
            }

            public void Init(IEnumerable<Request> requests)
            {
                foreach (IEvent request in requests)
                {
                    AddEvent(request);
                }
            }

            public void Clear()
            {
                _Events.Clear();
            }

            private class TimeComparer : Comparer<Seconds>
            {
                public override int Compare(Seconds x, Seconds y)
                {
                    return x.Value.CompareTo(y.Value);
                }
            }
        }

        #endregion
    }

    public struct SimulationState : ISimulationState
    {
        public IEvent CurrentEvent { get; }
        public Seconds CurrentTime { get; } 

        public SimulationState(IEvent currentEvent, Seconds currentTime)
        {
            CurrentEvent = currentEvent;
            CurrentTime = currentTime;
        }
    }

    public struct SimulationState<TEventType> : ISimulationState<TEventType> where TEventType : IEvent 
    {
        public TEventType Event { get; }
        public Seconds Time { get; }

        public SimulationState(TEventType currentEvent, Seconds currentTime)
        {
            Event = currentEvent;
            Time = currentTime;
        }
    }

    public class ElevatorEvent : IEvent
    {
        public Seconds WhenPlanned { get; }
        public Elevator Elevator { get; }
        public Floor EventLocation { get; }
        public ElevatorAction FinishedAction { get; }
        public List<Request> DepartedRequests { get; }
        public Centimeters Location => EventLocation.Location;

        public ElevatorEvent(Elevator elevator, Seconds whenPlanned, Floor eventLocation, ElevatorAction finishedAction, List<Request> departedRequests)
        {
            Elevator = elevator;
            WhenPlanned = whenPlanned;
            EventLocation = eventLocation;
            FinishedAction = finishedAction;
            DepartedRequests = departedRequests;
        }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Elevator: {Elevator}\n" +
            $"Destination: {EventLocation}";
    }
}
