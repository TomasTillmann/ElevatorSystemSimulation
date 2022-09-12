using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public class Simulation
    {
        private Calendar _Calendar { get; set; } = new();
        private bool _TerminateSimulation;
        private Seconds _LastStepTime = 0.ToSeconds();

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
            SetElevatorsIPlannableProperties();
        }

        public void Run()
        {
            foreach (IEvent request in Requests)
            {
                _Calendar.AddEvent(request);
            }

            while (CurrentTime < TotalTime && !_TerminateSimulation)
            {
                Step();
            }
        }

        public void Step()
        {
            IEvent? e = _Calendar.GetEvent();

            if(e == null)
            {
                _TerminateSimulation = true;
            }
            else
            {
                SetCurrentTime(e.WhenPlanned);
                SetElevatorsLocations(e);
                CurrentLogic.Step(e);
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
                ee.Elevator.Location = ee.CurrentFloor.Location;
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
            ElevatorEvent ee = new ElevatorEvent(elevator, CurrentTime + duration, destination, action);

            _Calendar.AddEvent(ee);
        }

        private void UnplanElevator(Elevator elevator)
        {
            //TODO - implement
        }

        #region Calendar

        private class Calendar
        {
            private PriorityQueue<IEvent, Seconds> Events { get; }

            public Calendar()
            {
                Events = new PriorityQueue<IEvent, Seconds>(new TimeComparer());
            }

            public IEvent? GetEvent()
            {
                if(Events.Count == 0)
                {
                    return null;
                }

                return Events.Dequeue();
            }

            public void AddEvent(IEvent e)
            {
                
                Events.Enqueue(e, e.WhenPlanned);
            }

            public void Clear()
            {
                Events.Clear();
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
        public Floor CurrentFloor { get; }
        public ElevatorAction FinishedAction { get; }

        public ElevatorEvent(Elevator elevator, Seconds whenPlanner, Floor destination, ElevatorAction finishedAction)
        {
            Elevator = elevator;
            WhenPlanned = whenPlanner;
            CurrentFloor = destination;
            FinishedAction = finishedAction;
        }

        public override string ToString() => 
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Elevator: {Elevator}\n" +
            $"Destination: {CurrentFloor}";
    }
}
