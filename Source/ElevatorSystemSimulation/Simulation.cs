using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public class Simulation : IRestartable
    {
        private Calendar _Calendar { get; set; } = new();
        private Seconds _LastStepTime = 0.ToSeconds();
        private bool _DidClientMadeAction;
        private Statistics Statistics;

        protected List<RequestEvent> _Requests;
        private List<RequestEvent> Requests
        {
            get => _Requests;
            set
            {
                _Requests = value;
                _Requests.Sort((RequestEvent r1, RequestEvent r2) => r1.WhenPlanned.Value.CompareTo(r2.WhenPlanned.Value));
                Restart();
            }
        }

        public Building Building { get => _Building; set { _Building = value; Restart(); } }
        private Building _Building;

        public Seconds CurrentTime { get; private set; } = 0.ToSeconds();
        public IElevatorLogic CurrentLogic { get; set; }
        public Seconds TotalTime { get; }
        public int StepCount { get; private set; }
        public IEvent? LastEvent { get; private set; }
        public IEvent? LastAction { get; private set; }
        public bool IsOver { get; private set; }

        public Simulation(
            Building building,
            IElevatorLogic currentLogic,
            Seconds totalTime,
            List<RequestEvent> requests)
        {
            CurrentLogic = currentLogic;
            _Requests = requests;
            Building = building;
            TotalTime = totalTime;

            _Calendar.Init(_Requests);

            SetElevatorsIPlannableProperties();

            if(requests.All(r => r.WhenPlanned == 0.ToSeconds()))
            {
                IsOver = true;
            }
        }

        public void Run()
        {
            while (CurrentTime < TotalTime && !IsOver)
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
                UpdateStateBeforeStep(e);
                CurrentLogic.Execute(new SimulationState(e, CurrentTime));
                UpdateStateAfterStep();
            }
            else
            {
                IsOver = true;
            }
        }

        public void Restart()
        {
            Building.ElevatorSystem.Value.ForEach(elevator => elevator.Restart());
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
            _Calendar.Init(_Requests);
        }

        private void SetCurrentTime(Seconds whenPlanned)
        {
            _LastStepTime = CurrentTime;
            CurrentTime = whenPlanned;
        }

        private void SetElevatorsLocations(IEvent e)
        {
            foreach(Elevator elevator in Building.ElevatorSystem.Value)
            {
                elevator.SetLocation(CurrentTime - _LastStepTime);
            }

            // This is necessary, because everything is rounded to seconds, hence the location might be a little bit off (this is the only affected place by rounding) - FIX? measure in milliseconds
            if(e is ElevatorEvent ee)
            {
                ee.Elevator.Location = ee.EventLocation.Location;
            }

            ValidateElevatorsLocations();
        }

        private void ValidateElevatorsLocations()
        {
            foreach(Elevator elevator in Building.ElevatorSystem.Value)
            {
                if(elevator.Location > Building.Floors.HeighestFloor.Location || 
                    elevator.Location < Building.Floors.LowestFloor.Location)
                {
                    throw new Exception("Elevators are out of the bounds of the building!");
                }
            }
        }

        private void SetElevatorsIPlannableProperties()
        {
            foreach (Elevator elevator in Building.ElevatorSystem.Value)
            {
                elevator.PlanElevator = PlanElevator;
                elevator.UnplanElevator = UnplanElevator;
            }
        }

        private void PlanElevator(Elevator elevator, Seconds duration, Floor destination, ElevatorAction action)
        {
            _DidClientMadeAction = true;
            ElevatorEvent ee = new ElevatorEvent(elevator, CurrentTime + duration, destination, action);

            LastAction = ee;

            if(ee.FinishedAction == ElevatorAction.MoveTo)
            {
                ee.EventLocation._PlannedElevators.Add(ee.Elevator);
            }

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

            if (e is ElevatorEvent ee)
            {
                if (ee.FinishedAction == ElevatorAction.UnloadAndLoad)
                {
                    ee.EventLocation._PlannedElevators.Remove(ee.Elevator);
                }
            }

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
        // Priority Queue is not sufficient, does not allow for removing specific events
        private class Calendar
        {
            private readonly List<(IEvent Event, Seconds WhenPlanned)> _Events = new();

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

            public void Init(IEnumerable<RequestEvent> requests)
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

    public struct SimulationState<EventType> : ISimulationState<EventType> where EventType : IEvent 
    {
        public EventType CurrentEvent { get; }
        public Seconds CurrentTime { get; }

        public SimulationState(EventType currentEvent, Seconds currentTime)
        {
            CurrentEvent = currentEvent;
            CurrentTime = currentTime;
        }
    }

    public class ElevatorEvent : IEvent
    {
        public Seconds WhenPlanned { get; }
        public Elevator Elevator { get; }
        public Floor EventLocation { get; }
        public ElevatorAction FinishedAction { get; }

        public Centimeters Location => EventLocation.Location;

        public ElevatorEvent(Elevator elevator, Seconds whenPlanner, Floor eventLocation, ElevatorAction finishedAction)
        {
            Elevator = elevator;
            WhenPlanned = whenPlanner;
            EventLocation = eventLocation;
            FinishedAction = finishedAction;
        }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Elevator: {Elevator}\n" +
            $"Destination: {EventLocation}";
    }
}
