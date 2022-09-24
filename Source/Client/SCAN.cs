using ElevatorSystemSimulation;

namespace Client
{
    public class SCAN : ElevatorLogic<BasicRequestEvent> 
    {
        // WIP
        public SCAN(Building building)
        :base(building)
        {
            _DoAfterElevatorAction.Add(ElevatorAction.MoveTo, StepAfterMove);
            _DoAfterElevatorAction.Add(ElevatorAction.UnloadAndLoad, StepAfterUnloadAndLoad);
            _DoAfterElevatorAction.Add(ElevatorAction.Idle, StepAfterIdle);
        }

        protected override void Step(BasicRequestEvent e)
        {
            Elevator? freeElevator = Elevators.Find(elevator => elevator.IsIdle);

            if(freeElevator != null)
            {
                freeElevator.MoveTo(e.EventLocation);
            }
            else
            {
                Elevators[0].MoveTo(e.EventLocation);
            }
        }

        protected override void Step(ElevatorEvent e)
        {
            _DoAfterElevatorAction[e.FinishedAction].Invoke(e);
        }

        private void StepAfterMove(ElevatorEvent e)
        {
            if(e.Elevator.AttendingRequests.Count > 0 || e.EventLocation.Requests.Count > 0)
            {
                e.Elevator.UnloadAndLoad(e.EventLocation);
            }
            else if(GetAllCurrentRequestEvents().Any())
            {
                e.Elevator.MoveTo(GetClosestFloorWithRequest(e.Elevator));
            }
            else
            {
                e.Elevator.Idle(e.EventLocation);

                Elevator? freeElevator = Elevators.Find(e => e.IsIdle);

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
                e.Elevator.Idle(e.EventLocation);

                Elevator? freeElevator = Elevators.Find(e => e.IsIdle);

                if(freeElevator != null)
                {
                    freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
                }
            }
        }

        private void StepAfterIdle(ElevatorEvent e)
        {
            Elevator? freeElevator = Elevators.Find(e => e.IsIdle);

            if(freeElevator != null)
            {
                freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
            }
        }

        private Floor GetClosestFloorWithRequest(Elevator elevator)
        {
            int minDistance = int.MaxValue;
            Floor floorToGo = null;

            foreach (BasicRequestEvent request in GetAllCurrentRequestEvents())
            {
                int distance = Math.Abs((request.EventLocation.Location - elevator.Location).Value);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    floorToGo = request.EventLocation;
                }
            }

            return floorToGo;
        }

        private Floor GetNextFloorByRequestToServe(ElevatorEvent e)
        {
            int minDistance = int.MaxValue;
            Floor floorToGo = null;

            foreach(BasicRequestEvent request in e.Elevator.AttendingRequests)
            {
                int distance = Math.Abs(request.Destination.Id - e.EventLocation.Id);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    floorToGo = request.Destination;
                }
            }

            return floorToGo;
        }
    }
}