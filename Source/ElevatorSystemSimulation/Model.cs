using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public class Elevator : IRestartable
    {
        #region Identification

        private static int _Counter = 0;
        public int Id { get; }

        #endregion

        #region Simulation

        public Action<Elevator, Seconds, Floor, ElevatorAction>? PlanElevator { get; set; }
        public Centimeters Location { get; set; }
        public Action<Elevator>? UnplanElevator { get; set; }
        public Direction Direction { get; private set; } = Direction.NoDirection;
        public Direction? LastDirection { get; private set; } = null; 
        public Floor? PlannedTo { get; private set; }

        public bool IsIdle => PlannedTo is null;

        public IReadOnlyCollection<IRequestEvent> AttendingRequests => _AttendingRequests;
        protected readonly List<IRequestEvent> _AttendingRequests  = new();

        public void Restart()
        {
            Location = 0.ToCentimeters();
            PlannedTo = null;
            Direction = Direction.NoDirection;
            LastDirection = null;
            _AttendingRequests.Clear();
        }

        #endregion

        #region Parameters

        public CentimetersPerSecond TravelSpeed { get; }
        public CentimetersPerSecond AccelerationDelaySpeed { get; }
        public Seconds DepartingTime { get; }
        public int Capacity { get; }

        #endregion

        public Elevator(
            CentimetersPerSecond travelSpeed,
            CentimetersPerSecond acceleratingTravelSpeed,
            Seconds departingTime,
            int capacity)
        {
            TravelSpeed = travelSpeed;
            AccelerationDelaySpeed = acceleratingTravelSpeed;
            DepartingTime = departingTime;
            Capacity = capacity;
            Location = 0.ToCentimeters();

            Id = _Counter++;
        }

        public void MoveTo(Floor? floor)
        {
            if (floor == null)
            {
                return;
            }

            if (!IsIdle)
            {
                UnplanMe();
            }

            PlannedTo = floor;

            LastDirection = Direction;
            Direction = (floor.Location - Location).Value > 0
                ? Direction.Up
                : Direction.Down;

            PlanMe(GetDistance(floor.Location) / TravelSpeed, floor, ElevatorAction.MoveTo);
        }

        public void Idle(Floor? floor)
        {
            if(floor == null)
            {
                return;
            }

            if (Location != floor.Location)
            {
                throw new Exception("Elevator cannot Idle. Elevators can Idle only when fully in the floor where the elevator currently is.");
            }

            if (!IsIdle)
            {
                UnplanMe();
            }

            PlannedTo = null;

            LastDirection = Direction;
            Direction = Direction.NoDirection;
            PlanMe(0.ToSeconds(), floor, ElevatorAction.Idle);
        }

        public void UnloadAndLoad(Floor? floor)
        {
            if(floor == null)
            {
                return;
            }

            if (Location != floor.Location)
            {
                throw new Exception("Elevator cannot load people. Elevators can load people only when fully in a floor.");
            }

            if (!IsIdle)
            {
                UnplanMe();
            }

            //TODO - IMPLEMENT: depart out + depart in time - maybe no one to depart out or no one to depart in on the floor 

            PlannedTo = floor;

            Unload(floor);
            Load(floor);

            LastDirection = Direction;
            Direction = Direction.NoDirection;
            PlanMe(DepartingTime, floor, ElevatorAction.UnloadAndLoad);
        }

        public override string ToString() => 
            $"ElevatorId: {Id}\n" +
            $"ElevatorLocation: {Location}";

        internal void SetLocation(Seconds stepDuration)
        {
            Location += Direction * (TravelSpeed * stepDuration);
        }

        private void Unload(Floor floor)
        {
            _AttendingRequests.RemoveAll(r => r.Destination == floor);
        }

        private void Load(Floor floor)
        {
            // implicitly adding requests that are the longest in the floor

            int numOfRemoved = 0;
            foreach(IRequestEvent request in floor.Requests)
            {
                if(_AttendingRequests.Count < Capacity)
                {
                    _AttendingRequests.Add(request);
                    numOfRemoved++;
                }
            }

            floor._Requests.RemoveRange(0, numOfRemoved);
        }

        private void PlanMe(Seconds duration, Floor destination, ElevatorAction action)
        {
            if (PlanElevator == null)
            {
                throw new Exception("Elevator cannot make this action. It is not in simulation yet.");
            }

            PlanElevator(this, duration, destination, action);
        }

        private void UnplanMe()
        {
            if (UnplanElevator == null)
            {
                throw new Exception("Elevator cannot make this action. It is not in simulation yet.");
            }

            UnplanElevator(this);
        }

        private Centimeters GetDistance(Centimeters floorLocation)
        {
            return new Centimeters(Math.Abs((floorLocation - Location).Value));
        }
    }

    public class Floor : IRestartable
    {
        #region Identification

        private static int _Counter = 0;

        public int Id { get; }
        public string? Name { get; }

        #endregion

        #region Simulation

        public IReadOnlyCollection<IRequestEvent> Requests => _Requests; 
        internal readonly List<IRequestEvent> _Requests = new();

        public IReadOnlyCollection<Elevator> PlannedElevators => _PlannedElevators;
        internal readonly List<Elevator> _PlannedElevators = new();

        public void Restart()
        {
            _Requests.Clear();
        }

        #endregion

        public Centimeters Height { get; }
        public Centimeters Location { get; set; }

        public Floor(Centimeters height, string? name = null)
        {
            Id = _Counter++;
            Height = height;
            Name = name;
        }

        public override string ToString() => 
            $"FloorId: {Id}\n" +
            $"FloorLocation: {Location}";
    }

    public class Building
    {
        public Floors Floors { get; set; }
        public ElevatorSystem ElevatorSystem { get; set; }

        public Building(Floors floors, ElevatorSystem elevatorSystem)
        {
            Floors = floors;
            ElevatorSystem = elevatorSystem;
        }
    }

    public class Floors
    {
        public List<Floor> Value { get; } = new();
        //public Centimeters InBetweenFloorsSpace { get; }

        public Floors(List<Floor> value, Centimeters inBetweenFloorsSpace)
        {
            //InBetweenFloorsSpace = inBetweenFloorsSpace;

            // avoiding sorting value too
            foreach(Floor floor in value)
            {
                Value.Add(floor);
            }
            //

            Value.Sort((f1, f2) => f1.Id.CompareTo(f2.Id));
            SetFloorsLocation();
        }

        public Floor? GetFloorById(int floorId)
        {
            return Value.Find(floor => floor.Id == floorId);
        }

        private void SetFloorsLocation()
        {
            Centimeters totalHeight = 0.ToCentimeters();

            foreach (Floor floor in Value)
            {
                floor.Location = totalHeight;
                totalHeight += floor.Height;
                //totalHeight += InBetweenFloorsSpace;
            }
        }
    }

    public class ElevatorSystem
    {
        public List<Elevator> Elevators { get; set; } = new();

        public ElevatorSystem(List<Elevator> elevators)
        {
            Elevators = elevators;
        }
    }
}