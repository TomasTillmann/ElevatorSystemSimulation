using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public class Simulation
    {
        private Calendar _Calendar { get; set; } = new();

        protected List<IRequestEvent> _Requests;
        public List<IRequestEvent> Requests
        {
            get => _Requests;
            set
            {
                _Requests = value;
                _Requests.Sort((IRequestEvent r1, IRequestEvent r2) => r1.WhenPlanned.Value.CompareTo(r2.WhenPlanned.Value));
            }
        }
        public Seconds CurrentTime { get; private set; }
        public Statistics Statistics { get; private set; } = new();
        public IElevatorLogic CurrentLogic { get; }
        public Building Building { get; }
        public Seconds TotalTime { get; }

        private bool _TerminateSimulation;

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

        private void Step()
        {
            IEvent? e = _Calendar.GetEvent();

            if(e == null)
            {
                _TerminateSimulation = true;
            }
            else
            {
                CurrentLogic.Step(e);
                _ResetAfterStep();
            }
        }

        private void _ResetAfterStep()
        {

        }

        private void SetElevatorsIPlannableProperties()
        {
            foreach (IPlannableElevator elevator in Building.ElevatorSystem.Elevators)
            {
                elevator.PlanElevator = PlanElevator;
                elevator.UnplanElevator = UnplanElevator;
            }
        }

        private void PlanElevator(IElevatorView elevator, Seconds duration, Floor destination)
        {
            _Calendar.AddEvent(new ElevatorEvent(elevator, CurrentTime + duration, destination));
        }

        private void UnplanElevator(IElevatorView elevator)
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

        #region ElevatorEvent

        private struct ElevatorEvent : IEvent
        {
            public Seconds WhenPlanned { get; }
            public IElevatorView Elevator { get; }
            public Floor Destination { get; }

            public ElevatorEvent(IElevatorView elevator, Seconds whenPlanner, Floor destination)
            {
                Elevator = elevator;
                WhenPlanned = whenPlanner;
                Destination = destination;
            }
        }

        #endregion
    }
}
