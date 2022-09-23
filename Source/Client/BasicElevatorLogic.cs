using ElevatorSystemSimulation;

namespace Client
{
    public class BasicElevatorLogic : ElevatorLogic<BasicRequestEvent> 
    {
        private List<Elevator> _Elevators { get; set; }
        private Floors _Floors { get; set; }
        private Random _Random { get; }
        private readonly Dictionary<ElevatorAction, Action<ElevatorEvent>> _DoAfterElevatorAction = new();

        public BasicElevatorLogic(Building building)
        :base(building)
        {
            _Elevators = building.ElevatorSystem.Elevators;
            _Floors = building.Floors;
            _Random = new Random();

            _DoAfterElevatorAction.Add(ElevatorAction.MoveTo, StepAfterMove);
            _DoAfterElevatorAction.Add(ElevatorAction.UnloadAndLoad, StepAfterUnloadAndLoad);
            _DoAfterElevatorAction.Add(ElevatorAction.Idle, StepAfterIdle);
        }

        protected override void Step(BasicRequestEvent e)
        {
            Elevator? freeElevator = _Elevators.Find(elevator => elevator.IsIdle);

            if(freeElevator != null)
            {
                freeElevator.MoveTo(e.Floor);
            }
        }

        protected override void Step(ElevatorEvent e)
        {
            _DoAfterElevatorAction[e.FinishedAction].Invoke(e);
        }

        //TODO - refactor and implement and run and test ...
        private void StepAfterMove(ElevatorEvent e)
        {
            if(e.Elevator.AttendingRequests.Count > 0 || e.CurrentFloor.Requests.Count > 0)
            {
                e.Elevator.UnloadAndLoad(e.CurrentFloor);
            }
            else if(GetAllCurrentRequestEvents().Any())
            {
                e.Elevator.MoveTo(GetClosestFloorWithRequest(e.Elevator));
            }
            else
            {
                e.Elevator.Idle(e.CurrentFloor);

                Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

                if(freeElevator != null)
                {
                    freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
                }
            }
        }

        private void StepAfterUnloadAndLoad(ElevatorEvent e)
        {
            if(e.Elevator.AttendingRequests.Count > 0)
            {
                // serve the requests
                Floor  floor = GetNextFloorByRequestToServe(e);
                e.Elevator.MoveTo(floor);
            }
            else if (GetAllCurrentRequestEvents().Any())
            {
                Floor floor = GetClosestFloorWithRequest(e.Elevator);
                e.Elevator.MoveTo(floor);
            }
            else
            {
                e.Elevator.Idle(e.CurrentFloor);

                Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

                if(freeElevator != null)
                {
                    freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
                }
            }
        }

        private void StepAfterIdle(ElevatorEvent e)
        {
            Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

            if(freeElevator != null)
            {
                freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
            }
        }

        private Floor GetClosestFloorWithRequest(Elevator elevator)
        {
            int minDistance = int.MaxValue;
            Floor floorToGo = null;

            // it will choose the closest floor - hungry approach
            foreach (BasicRequestEvent request in GetAllCurrentRequestEvents())
            {
                int distance = Math.Abs((request.Floor.Location - elevator.Location).Value);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    floorToGo = request.Floor;
                }
            }

            return floorToGo;
        }

        private Floor GetNextFloorByRequestToServe(ElevatorEvent e)
        {
            int minDistance = int.MaxValue;
            Floor floorToGo = null;

            // it will choose the closest floor - hungry approach
            foreach(BasicRequestEvent request in e.Elevator.AttendingRequests)
            {
                int distance = Math.Abs(request.Destination.FloorId - e.CurrentFloor.FloorId);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    floorToGo = request.Destination;
                }
            }

            return floorToGo;
        }
        //
    }
}