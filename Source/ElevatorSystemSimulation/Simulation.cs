using DataTypes;
using Interfaces;
using Model;

namespace ElevatorSystemSimulation
{
    public class Simulation
    {
        private Calendar _Calendar { get; set; } = new();

        protected List<IRequest> _Requests;
        public List<IRequest> Requests
        {
            get => _Requests;
            set
            {
                _Requests = value;
                _Requests.Sort((IRequest r1, IRequest r2) => r1.WhenPlanned.Value.CompareTo(r2.WhenPlanned.Value));
            }
        }
        public Seconds CurrentTime { get; private set; }
        public Statistics _Statistics { get; private set; } = new();
        public IElevatorLogic CurrentLogic { get; }
        public Building Building { get; }
        public Seconds TotalTime { get; }

        private bool _TerminateSimulation;

        public Simulation(
            Building building,
            IElevatorLogic currentLogic,
            Seconds totalTime,
            List<IRequest> requests)
        {
            CurrentLogic = currentLogic;
            Building = building;
            TotalTime = totalTime;

            _Requests = requests; 
            SetElevatorsIPlannableProperties();
        }

        public void Run()
        {
            foreach (IRequest request in Requests)
            {
                _Calendar.AddEvent(
                    new RequestEvent(
                        request.WhenPlanned,
                        request
                        )
                );
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

        private void PlanElevator(IElevator elevator, Seconds duration, Centimeters location)
        {
            _Calendar.AddEvent(new ElevatorEvent(elevator, CurrentTime + duration, location));
        }

        private void UnplanElevator(IElevator elevator)
        {

        }
    }

    public struct ElevatorEvent : IEvent
    {
        public Seconds WhenPlanned { get; }
        public IElevator Elevator { get; }
        public Centimeters Destination { get; }

        public ElevatorEvent(IElevator elevator, Seconds whenPlanner, Centimeters destination)
        {
            Elevator = elevator;
            WhenPlanned = whenPlanner;
            Destination = destination;
        }
    }

    public struct RequestEvent : IEvent
    {
        public Seconds WhenPlanned { get; }
        public IRequest Request { get; }

        public RequestEvent(Seconds whenPlanned, IRequest request)
        {
            WhenPlanned = whenPlanned;
            Request = request;
        }
    }

    public struct Request : IRequest
    {
        public Floor Floor { get; }
        public Seconds WhenPlanned { get; }

        //public int ProbabilityToGenerate { get; set; }
        //public int? NumberOfPersons { get; set; }
        //public Floor? ToFloor { get; set; }
        //public int? Priority { get; set; }
        //public List<int>? AllowedElevators { get; set; }

        public Request(Floor floor, Seconds whenPlanned)
        {
            Floor = floor;
            WhenPlanned = whenPlanned;
        }
    }

    internal class Calendar
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

    public class Generator
    {
        private Building _Building { get; set; }
        public Generator(Building building)
        {
            _Building = building;
        }

        public List<IRequest> GenerateRequests()
        {
            return new();
        }
    }
}
