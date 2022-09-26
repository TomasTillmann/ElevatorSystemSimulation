using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;

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

                // elevators heading in the same direction as this current request - avoids change of direction of the elevator
                elevator => (elevator.Location >= e.EventLocation.Location && elevator.Direction != Direction.Up ||
                elevator.Location <= e.EventLocation.Location && elevator.Direction != Direction.Down) &&

                // implication - this request in between the planned one and the elevator
                (elevator.PlannedTo == null ||
                GetDistance(elevator.PlannedTo, elevator) > GetDistance(e.EventLocation, elevator)));

            if(elevators.Count == 0)
            {
                elevators = GetClosestElevators(e.EventLocation, e => e.IsIdle);

                if(elevators.Count == 0)
                {
                    // if all elevators are busy, don't do anything
                    return;
                }
            }
            else
            {
                int minPeopleCount = int.MaxValue;
                elevators.ForEach(e => minPeopleCount = e.AttendingRequests.Count < minPeopleCount ? e.AttendingRequests.Count : minPeopleCount);
                elevators.Where(e => e.AttendingRequests.Count == minPeopleCount).First().MoveTo(e.EventLocation);
            }
        }

        protected override void Step(ElevatorEvent e)
        {
            _DoAfterElevatorAction[e.FinishedAction].Invoke(e);
        }

        private void StepAfterFloorArrival(ElevatorEvent e)
        {
            if( e.EventLocation.Requests.Count > 0)
            {
                e.Elevator.UnloadAndLoad(e.EventLocation);
            }
            else if(e.Elevator.AttendingRequests.Count > 0)
            {
                Floor? floor = GetNextFloorByRequestToServe(e);
                e.Elevator.MoveTo(floor);
            }
            else if(GetAllCurrentRequestEvents().Any())
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, floorFilter : f => f.PlannedElevators.Count == 0);
                if (floors.Count == 0)
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator); 

                    if(floors.Count == 0)
                    {
                        e.Elevator.Idle(e.EventLocation);
                    }
                    else
                    {
                        e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                    }
                }
                else
                {
                    e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                }
            }
            else
            {
                e.Elevator.Idle(e.EventLocation);
            }
        }

        private void StepAfterUnloadAndLoad(ElevatorEvent e)
        {
            if (e.Elevator.AttendingRequests.Count > 0)
            {
                Floor? floor = GetNextFloorByRequestToServe(e);
                e.Elevator.MoveTo(floor);
            }
            else if (GetAllCurrentRequestEvents().Any())
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, floorFilter: f => f.PlannedElevators.Count == 0);
                if (floors.Count == 0)
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator);

                    if (floors.Count == 0)
                    {
                        e.Elevator.Idle(e.EventLocation);
                    }
                    else
                    {
                        e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                    }
                }
                else
                {
                    e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                }
            }
            else
            {
                e.Elevator.Idle(e.EventLocation);
            }
        }

        private void StepAfterIdle(ElevatorEvent e)
        {
            if (GetAllCurrentRequestEvents().Any())
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, floorFilter: f => f.PlannedElevators.Count == 0);
                if (floors.Count == 0)
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator);

                    if (floors.Count == 0)
                    {
                        e.Elevator.Idle(e.EventLocation);
                    }
                    else
                    {
                        e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                    }
                }
                else
                {
                    e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                }
            }
            else
            {
                e.Elevator.Idle(e.EventLocation);
            }
        }

        #region HelpingMehotds

        private List<Floor> GetFloorsWithMaxWaitingTimeOnRequest(List<Floor>? floors = null)
        {
            floors = floors ?? Floors;

            Seconds maxWaitingTime = 0.ToSeconds();
            List<Floor> floorsWithMaxWaitingTimeOnRequest = new();
            foreach(Floor floor in floors)
            {
                foreach(BasicRequestEvent request in floor.Requests)
                {
                    if(CurrentTime - request.WhenPlanned > maxWaitingTime)
                    {
                        maxWaitingTime = CurrentTime - request.WhenPlanned;
                        floorsWithMaxWaitingTimeOnRequest.Clear();
                        floorsWithMaxWaitingTimeOnRequest.Add(floor);
                    }
                    else if(CurrentTime - request.WhenPlanned == maxWaitingTime)
                    {
                        floorsWithMaxWaitingTimeOnRequest.Add(floor);
                    }
                }
            }

            return floorsWithMaxWaitingTimeOnRequest;
        }

        private Floor? GetNextFloorByRequestToServe(ElevatorEvent e)
        {
            if(e.Elevator.LastDirection == Direction.Up)
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, requestFilter: r => (r.EventLocation.Location - e.Elevator.Location).Value > 0);
                if (floors.Count == 0)
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator);

                    if (floors.Count == 0)
                    {
                        e.Elevator.Idle(e.EventLocation);
                    }
                    else
                    {
                        e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                    }
                }
                return floors.FirstOrDefault();
            }
            else if(e.Elevator.LastDirection == Direction.Down)
            {
                List<Floor> floors = GetClosestFloorsWithRequest(e.Elevator, requestFilter: r => (r.EventLocation.Location - e.Elevator.Location).Value < 0);
                if (floors.Count == 0)
                {
                    floors = GetClosestFloorsWithRequest(e.Elevator);

                    if (floors.Count == 0)
                    {
                        e.Elevator.Idle(e.EventLocation);
                    }
                    else
                    {
                        e.Elevator.MoveTo(GetFloorsWithMaxWaitingTimeOnRequest(floors).First());
                    }
                }
                return floors.FirstOrDefault();
            }
            else
            {
                throw new Exception("Last action must be either up or down, because now it is no direction and the elevator had to get in this position somehow.");
            }
        }
    }

    #endregion
}