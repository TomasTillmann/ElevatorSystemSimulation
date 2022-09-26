using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public class Simulation : IRestartable
    {
        private Calendar _Calendar { get; set; } = new();
        private Seconds _LastStepTime = 0.ToSeconds();
        private bool _DidClientMadeAction;

        protected List<IRequestEvent> _Requests;
        private List<IRequestEvent> Requests
        {
            get => _Requests;
            set
            {
                _Requests = value;
                _Requests.Sort((IRequestEvent r1, IRequestEvent r2) => r1.WhenPlanned.Value.CompareTo(r2.WhenPlanned.Value));
            }
        }

        public Seconds CurrentTime { get; private set; } = 0.ToSeconds();
        public IElevatorLogic CurrentLogic { get; }
        public Building Building { get; }
        public Seconds TotalTime { get; }
        public int StepCount { get; private set; }
        public IEvent? LastEvent { get; private set; }
        public IEvent? LastAction { get; private set; }
        public bool IsOver { get; private set; }

        public Simulation(
            Building building,
            IElevatorLogic currentLogic,
            Seconds totalTime,
            List<IRequestEvent> requests)
        {
            CurrentLogic = currentLogic;
            Building = building;
            TotalTime = totalTime;

            _Requests = requests;

            _Calendar.Init(_Requests);
            SetElevatorsIPlannableProperties();
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
                CurrentLogic.Step(e, CurrentTime);
                UpdateStateAfterStep();
            }
            else
            {
                IsOver = true;
            }
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
            _Calendar.Init(_Requests);
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

            public void Init(IEnumerable<IRequestEvent> requests)
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

    public struct ElevatorEvent : IEvent
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
