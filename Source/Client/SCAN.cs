using ElevatorSystemSimulation;

namespace Client
{
    public class SCAN : ElevatorLogic<BasicRequestEvent> 
    {
        // WIP
        public SCAN(Building building)
        :base(building)
        {
            _DoAfterElevatorAction.Add(ElevatorAction.MoveTo, StepAfterFloorArrival);
            _DoAfterElevatorAction.Add(ElevatorAction.UnloadAndLoad, StepAfterUnloadAndLoad);
            _DoAfterElevatorAction.Add(ElevatorAction.Idle, StepAfterIdle);
        }

        protected override void Step(BasicRequestEvent e)
        {
            List<Elevator> elevators = GetClosestElevators(e.EventLocation, filter:
                elevator => (elevator.Location >= e.EventLocation.Location && elevator.Direction != Direction.Up ||
                elevator.Location <= e.EventLocation.Location && elevator.Direction != Direction.Down) &&

                // implication
                (elevator.PlannedTo == null ||
                Math.Abs((elevator.PlannedTo.Location - elevator.Location).Value) > 
                Math.Abs((e.EventLocation.Location - elevator.Location).Value)));

            if(elevators.Count == 0)
            {
                elevators = GetClosestElevators(e.EventLocation, e => e.IsIdle);
            }

            if(elevators.Count == 0)
            {
                return;
            }

            elevators.First().MoveTo(e.EventLocation);
        }

        protected override void Step(ElevatorEvent e)
        {
            _DoAfterElevatorAction[e.FinishedAction].Invoke(e);
        }

        private void StepAfterFloorArrival(ElevatorEvent e)
        {
            if(e.Elevator.AttendingRequests.Count > 0 || e.EventLocation.Requests.Count > 0)
            {
                e.Elevator.UnloadAndLoad(e.EventLocation);
            }
            else if(GetAllCurrentRequestEvents().Any())
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, floorFilter : f => f.PlannedElevators.Count == 0);
                if (!floors.Any())
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator); //TODO - requests that wait the longest
                }

                e.Elevator.MoveTo(floors.First());
            }
            else
            {
                e.Elevator.Idle(e.EventLocation);
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
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, floorFilter: f => f.PlannedElevators.Count == 0);
                if (!floors.Any())
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator);
                }

                e.Elevator.MoveTo(floors.First());
            }
            else
            {
                e.Elevator.Idle(e.EventLocation);
            }
        }

        private void StepAfterIdle(ElevatorEvent e)
        {
            e.Elevator.MoveTo(GetClosestFloorsWithRequest(e.Elevator).First());
        }


        private Floor GetNextFloorByRequestToServe(ElevatorEvent e)
        {
            if(e.Elevator.LastDirection == Direction.Up)
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, requestFilter: r => (r.EventLocation.Location - e.Elevator.Location).Value > 0);
                if (!floors.Any())
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator); //TODO - requests that wait the longest
                }
                return floors.First();
            }
            else if(e.Elevator.LastDirection == Direction.Down)
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, requestFilter: r => (r.EventLocation.Location - e.Elevator.Location).Value < 0);
                if (!floors.Any())
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator); //TODO - requests that wait the longest
                }
                return floors.First();
            }
            else
            {
                throw new Exception("Last action must be either up or down, because now it is no direction and the elevator had to get in this position somehow.");
            }
        }
    }
}