using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public class Elevator : IRestartable, ILocatable, IIdentifiable
    {
        #region Identification

        public int Id { get; internal set; }

        #endregion

        #region Simulation

        public Centimeters Location { get; set; }
        public Direction Direction { get; private set; } = Direction.NoDirection;
        public Direction? LastDirection { get; private set; } = null; 
        public Floor? PlannedTo { get; private set; }

        public bool IsIdle => PlannedTo is null;

        public IReadOnlyCollection<Interfaces.Request> AttendingRequests => _AttendingRequests;
        protected readonly List<Interfaces.Request> _AttendingRequests  = new();

        internal Action<Elevator, Seconds, Floor, ElevatorAction>? PlanElevator { get; set; }
        internal Action<Elevator>? UnplanElevator { get; set; }

        internal ElevatorInfo? Info { get; set; } = new(0.ToSeconds(), 0, 0);

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

            // Dont care about this for now - TODO
            AccelerationDelaySpeed = acceleratingTravelSpeed;
            //

            DepartingTime = departingTime;
            Capacity = capacity;
            Location = 0.ToCentimeters();
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
                throw new Exception("Elevator cannot load and unload people. Elevators can load and unload people only when fully in a floor.");
            }

            if (!IsIdle)
            {
                UnplanMe();
            }

            PlannedTo = floor;

            Unload(floor);
            Load(floor);

            LastDirection = Direction;
            Direction = Direction.NoDirection;
            PlanMe(DepartingTime, floor, ElevatorAction.UnloadAndLoad);
        }

        /// <summary>
        /// Loads only the specified requests in order given by the enumerable. If not specified, it follows the order of Requests. In that case, it implicitly adds requests that are the longest in the floor.
        /// Checks for the capacity. If Capacity of the elevator is full, no more requests are added to the elevator and they are left on the floor.
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="requests"></param>
        public void Load(Floor? floor, IEnumerable<Interfaces.Request>? requests = null)
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

            requests = requests == null ? floor.Requests : requests;

            HashSet<Interfaces.Request> requestsToDelete = new();
            foreach(Interfaces.Request request in requests)
            {
                if(_AttendingRequests.Count < Capacity)
                {
                    _AttendingRequests.Add(request);
                    requestsToDelete.Add(request);
                }
            }

            floor._Requests.RemoveAll(r => requestsToDelete.Contains(r));

            PlannedTo = floor;
            LastDirection = Direction;
            Direction = Direction.NoDirection;
            PlanMe(DepartingTime, floor, ElevatorAction.Load);
        }

        /// <summary>
        /// Unloads all the people that want to get out at this floor.
        /// </summary>
        /// <param name="floor"></param>
        public void Unload(Floor? floor)
        {
            if(floor == null)
            {
                return;
            }

            if (Location != floor.Location)
            {
                throw new Exception("Elevator cannot unload people. Elevators can unload people only when fully in a floor.");
            }

            if (!IsIdle)
            {
                UnplanMe();
            }

            _AttendingRequests.RemoveAll(r => r.Destination == floor);

            PlannedTo = floor;
            LastDirection = Direction;
            Direction = Direction.NoDirection;
            PlanMe(DepartingTime, floor, ElevatorAction.Unload);
        }

        public override string ToString() => 
            $"ElevatorId: {Id}\n" +
            $"ElevatorLocation: {Location}";

        internal void SetLocation(Seconds stepDuration)
        {
            Location += Direction * (TravelSpeed * stepDuration);
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

    public class Floor : IRestartable, ILocatable
    {
        #region Identification
        public int Id { get; internal set; }
        public string? Name { get; }

        #endregion

        #region Simulation

        public IReadOnlyCollection<Request> Requests => _Requests; 
        internal readonly List<Request> _Requests = new();

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
        public Floor HeighestFloor => Value[Value.Count - 1];
        public Floor LowestFloor => Value[0]; 

        //public Centimeters InBetweenFloorsSpace { get; }

        public Floors(List<Floor> value, Centimeters inBetweenFloorsSpace)
        {
            //InBetweenFloorsSpace = inBetweenFloorsSpace;

            Value = new List<Floor>(value);
            Value.Sort((f1, f2) => f1.Id.CompareTo(f2.Id));

            int i = -1;
            Value.ForEach(f => f.Id = i+=1);

            SetFloorsLocation();
        }

        public Floor? GetFloorById(int floorId)
        {
            return Value.Find(floor => floor.Id == floorId);
        }

        public IEnumerable<Interfaces.Request> GetAllActiveRequests()
        {
            foreach(Floor floor in Value)
            {
                foreach(Interfaces.Request request in floor.Requests)
                {
                    yield return request;
                }
            }
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

        public ElevatorSystem(List<Elevator> value)
        {
            Elevators = value;

            int i = -1;
            value.ForEach(e => e.Id = i += 1);
        }

        public IEnumerable<Request> GetAllInElevatorRequests()
        {
            foreach(Elevator elevator in Elevators)
            {
                foreach(Request request in elevator.AttendingRequests)
                {
                    yield return request;
                }
            }
        }

        internal void ValidateElevatorsLocations(Floors floors)
        {
            foreach (Elevator elevator in Elevators)
            {
                if (elevator.Location > floors.HeighestFloor.Location ||
                    elevator.Location < floors.LowestFloor.Location)
                {
                    throw new Exception("Elevators are out of the bounds of the building!");
                }
            }
        }
    }
}