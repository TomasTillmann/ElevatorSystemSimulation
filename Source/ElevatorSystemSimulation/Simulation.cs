using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public sealed class Simulation<TRequest> : IRestartable, ISimulation<TRequest> where TRequest : Request
    {
        private Calendar _Calendar { get; set; } = new();
        private Seconds _LastStepTime = 0.ToSeconds();
        private bool _DidClientMadeAction;
        private Statistics<TRequest> Statistics = new();

        public List<TRequest> _AllRequests;
        public List<TRequest> _DepartedRequests = new();

        public IReadOnlyList<TRequest> AllRequests => _AllRequests;
        public IReadOnlyList<TRequest> DepartedRequests => _DepartedRequests;

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

        List<Request> CurrentlyDepartedRequests = new();

        public Simulation(
            Building building,
            IElevatorLogic<TRequest> currentLogic,
            List<TRequest> requests)
        {
            CurrentLogic = currentLogic;
            _AllRequests = requests;
            _Building = building;

            _Calendar.Init(_AllRequests);

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
            if(_DepartedRequests.Count == AllRequests.Count)
            {
                IsOver = true;
            }

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
            return Statistics.GetResult(_AllRequests.Select(r => (Request)r).ToList(), ElevatorSystem.Elevators);
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

            //restart requests
            _DepartedRequests.Clear();
            //

            _Calendar.Clear();
            _Calendar.Init(_AllRequests);
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
                elevator.AfterStepStateUpdate = AfterStepStateUpdate;
            }
        }

        private void AfterStepStateUpdate(List<Request> servedRequests)
        {
            foreach(Request req in servedRequests)
            {
                //TODO: the cast is super ugly. Making Elevator abstract and parametrizing it should be a better idea and it could fix this.
                _DepartedRequests.Add((TRequest)req);
                CurrentlyDepartedRequests.Add(req);
            }
        }

        private void PlanElevator(Elevator elevator, Seconds duration, Floor destination, ElevatorAction action)
        {
            _DidClientMadeAction = true;

            if(action == ElevatorAction.MoveTo)
            {
                //TODO: also had to remove elevators from planned elevators
                destination._PlannedElevators.Add(elevator);
            }

            ElevatorEvent ee = new ElevatorEvent(elevator, CurrentTime + duration, destination, action, CurrentlyDepartedRequests);

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
            CurrentlyDepartedRequests.Clear();
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
}
