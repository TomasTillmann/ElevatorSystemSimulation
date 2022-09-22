using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public class Elevator
    {
        #region Identification

        private static int Counter = 0;
        public int Id { get; }

        #endregion

        #region SimulationPlanning

        public Action<Elevator, Seconds, Floor, ElevatorAction>? PlanElevator { get; set; }
        public Action<Elevator>? UnplanElevator { get; set; }
        public bool IsIdle { get; private set; } = true; 
        public Direction Direction { get; private set; } = Direction.NoDirection;
        public Centimeters Location { get; set; }
        public IReadOnlyCollection<IRequestEvent> AttendingRequests => _AttendingRequests;
        protected readonly List<IRequestEvent> _AttendingRequests  = new();

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
            int capacity,
            Floor? startingFloor)
        {
            TravelSpeed = travelSpeed;
            AccelerationDelaySpeed = acceleratingTravelSpeed;
            DepartingTime = departingTime;
            Capacity = capacity;
            Location = startingFloor != null ? startingFloor.Location : 0.ToCentimeters();

            Id = ++Counter;
        }

        public void MoveTo(Floor? floor)
        {
            if (floor == null)
            {
                return;
            }

            IsIdle = false;

            SetDirection(floor.Location);
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

            IsIdle = true;


            PlanMe(0.ToSeconds(), floor, ElevatorAction.Idle);
        }

        public void UnloadAndLoad(Floor? floor)
        {
            //TODO - implement loading - need to delete requests that were served by loading
            if(floor == null)
            {
                return;
            }

            if (Location != floor.Location)
            {
                throw new Exception("Elevator cannot load people. Elevators can load people only when fully in a floor.");
            }

            //TODO - IMPLEMENT: depart out + depart in time - maybe no one to depart out or no one to depart in on the floor 

            IsIdle = false;

            Unload(floor);
            Load(floor);

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

            foreach(IRequestEvent request in floor.Requests)
            {
                if(_AttendingRequests.Count < Capacity)
                {
                    _AttendingRequests.Add(request);
                }
            }
        }

        private void PlanMe(Seconds duration, Floor destination, ElevatorAction action)
        {
            if (PlanElevator == null)
            {
                throw new Exception("Elevator cannot make this action. It is not in simulation yet.");
            }

            PlanElevator(this, duration, destination, action);
        }

        private void UnplanMe(Seconds duration, Centimeters location)
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

        private void SetDirection(Centimeters floorLocation)
        {
            Direction = (floorLocation - Location).Value > 0 ?
                Direction.Up :
                Direction.Down;
        }
    }

    public class Floor
    {
        public Centimeters Location { get; set; }
        public int FloorId { get; }
        public Centimeters Height { get; }
        public IReadOnlyCollection<IRequestEvent> Requests => _Requests; 
        internal List<IRequestEvent> _Requests { get; } = new();
        public string? Name { get; }

        public Floor(int floorId, Centimeters height, string? name = null)
        {
            FloorId = floorId;
            Height = height;
            Name = name;
        }

        public override string ToString() => 
            $"FloorId: {FloorId}\n" +
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

            Value.Sort((f1, f2) => f1.FloorId.CompareTo(f2.FloorId));
            SetFloorsLocation();
        }

        public Floor? GetFloorById(int floorId)
        {
            return Value.Find(floor => floor.FloorId == floorId);
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