using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;
using System.Drawing;

namespace ElevatorSystemSimulation
{
    public class Elevator : IRestartable, ILocatable, IIdentifiable
    {
        #region Identification

        public int Id { get { return _Id; } internal set { _Id = value; Info.ElevatorId = value; } }
        private int _Id;

        #endregion

        #region Simulation

        public Centimeters Location { get; set; }
        public Direction Direction { get; private set; } = Direction.NoDirection;
        public Direction? LastDirection { get; private set; } = null; 
        public Floor? PlannedTo { get; private set; }

        public bool IsIdle => PlannedTo is null;

        public IReadOnlyCollection<Request> AttendingRequests => _AttendingRequests;
        protected readonly List<Request> _AttendingRequests  = new();

        internal Action<List<Request>>? AfterStepStateUpdate { get; set; }
        internal Action<Elevator, Seconds, Floor, ElevatorAction>? PlanElevator { get; set; }
        internal Action<Elevator>? UnplanElevator { get; set; }

        internal ElevatorInfo Info { get; set; }

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

        private IEnumerable<Request>? DepartRequests;

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

            Info = new(0.ToSeconds(), 0, 0);
        }

        public void MoveTo(Floor floor)
        {
            if (floor is null) return;
            Replan();

            PlannedTo = floor;
            LastDirection = Direction;
            Direction = (floor.Location - Location).Value > 0
                ? Direction.Up
                : Direction.Down;

            PlanMe(GetDistance(floor.Location) / TravelSpeed, floor, ElevatorAction.MoveTo);
        }

        public void Idle(Floor floor)
        {
            if (floor is null) return;
            IsFullyInFloor(floor);
            Replan();

            PlannedTo = null;
            LastDirection = Direction;
            Direction = Direction.NoDirection;

            PlanMe(0.ToSeconds(), floor, ElevatorAction.Idle);
        }

        public void UnloadAndLoad(Floor floor)
        {
            if (floor is null) return;
            IsFullyInFloor(floor);
            Replan();

            PlannedTo = floor;
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
        public void Load(Floor floor, IEnumerable<Request>? requests = null)
        {
            if (floor is null) return;
            IsFullyInFloor(floor);
            Replan();

            DepartRequests = requests ?? floor.Requests;
            PlannedTo = floor;
            LastDirection = Direction;
            Direction = Direction.NoDirection;

            PlanMe(DepartingTime, floor, ElevatorAction.Load);
        }

        /// <summary>
        /// Unloads all the people that want to get out at this floor.
        /// </summary>
        /// <param name="floor"></param>
        public void Unload(Floor floor)
        {
            if (floor is null) return;
            IsFullyInFloor(floor);
            Replan();

            PlannedTo = floor;
            LastDirection = Direction;
            Direction = Direction.NoDirection;

            PlanMe(DepartingTime, floor, ElevatorAction.Unload);
        }

        internal void UnloadAndLoadAction()
        {
            UnloadFloorRequests(PlannedTo!);
            LoadFloorRequests(PlannedTo!, PlannedTo!.Requests);
        }

        internal void UnloadAction()
        {
            UnloadFloorRequests(PlannedTo!);
        }

        internal void LoadAction()
        {
            LoadFloorRequests(PlannedTo!, PlannedTo!.Requests);
        }

        private void IsFullyInFloor(Floor floor)
        {
            if (Location != floor.Location)
            {
                throw new Exception("Elevator cannot be planned with this action when not fully in the floor.");
            }
        }

        /// Unplans the last planned action. This allows for replanning. For example, if elevator in previous step was planned to move to floor 7, but now, it is planned to floor 5, it will not go to floor 7 and go to floor 5 only. Deleting the move to floor 7 plan entirely.
        private void Replan()
        {
            if (!IsIdle)
            {
                UnplanMe();
            }
        }

        private void LoadFloorRequests(Floor floor, IEnumerable<Request> requests)
        {
            HashSet<Request> requestsToDelete = new();
            foreach (Request request in requests)
            {
                if (_AttendingRequests.Count < Capacity)
                {
                    _AttendingRequests.Add(request);
                    requestsToDelete.Add(request);
                }
            }

            floor._Requests.RemoveAll(r => requestsToDelete.Contains(r));
        }

        private void UnloadFloorRequests(Floor floor)
        {
            List<Request> servedRequests = _AttendingRequests.Where(r => r.Destination == floor).ToList();
            if (!servedRequests.Any())
            {
                return;
            }

            _AttendingRequests.RemoveAll(r => r.Destination == floor);

            if(AfterStepStateUpdate == null)
            {
                throw new Exception("Elevator cannot update simulation state. It is not in simulation yet.");
            }

            AfterStepStateUpdate.Invoke(servedRequests);
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
}