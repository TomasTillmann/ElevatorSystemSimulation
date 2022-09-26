using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public abstract class ElevatorLogic<RequestEvent> : IElevatorLogic where RequestEvent : IRequestEvent
    {
        protected Building Building { get; }
        protected List<Elevator> Elevators => Building.ElevatorSystem.Elevators;
        protected List<Floor> Floors => Building.Floors.Value;
        protected Dictionary<ElevatorAction, Action<ElevatorEvent>> _DoAfterElevatorAction { get; } = new();
        protected Seconds CurrentTime { get; private set; }

        public ElevatorLogic(Building building)
        {
            Building = building;
        }

        public void Step(IEvent e, Seconds currentTime)
        {
            CurrentTime = currentTime;

            if(e is RequestEvent ce)
            {
                ce.EventLocation._Requests.Add(ce);
                Step(ce);
            }
            else if(e is ElevatorEvent ee)
            {
                Step(ee);
            }
            else
            {
                throw new Exception("Event is something different.");
            }
        }

        protected abstract void Step(RequestEvent ce);
        protected abstract void Step(ElevatorEvent ee);

        protected Centimeters GetDistance(ILocatable l1, ILocatable l2)
        {
            return Math.Abs((l1.Location - l2.Location).Value).ToCentimeters();
        }

        protected virtual IEnumerable<RequestEvent> GetAllCurrentRequestEvents(Predicate<Floor>? filter = null)
        {
            filter = filter ?? new Predicate<Floor>(f => true);

            foreach(Floor floor in Building.Floors.Value.Where(f => filter(f)))
            {
                foreach(RequestEvent requestEvent in floor.Requests)
                {
                    yield return requestEvent;
                }
            }
        }

        protected virtual List<Floor> GetClosestFloorsWithRequest(Elevator elevator, Predicate<Floor>? floorFilter = null, Predicate<RequestEvent>? requestFilter = null)
        {
            floorFilter = floorFilter ?? new Predicate<Floor>(f => true);
            requestFilter = requestFilter ?? new Predicate<RequestEvent>(r => true);

            int minDistance = int.MaxValue;
            List<Floor> closestFloors = new();

            foreach (RequestEvent request in GetAllCurrentRequestEvents().Where(r => requestFilter(r)))
            {
                if (!floorFilter(request.EventLocation))
                {
                    continue;
                }

                int distance = Math.Abs((request.EventLocation.Location - elevator.Location).Value);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestFloors.Clear();
                    closestFloors.Add(request.EventLocation);
                }
                else if(distance == minDistance)
                {
                    closestFloors.Add(request.EventLocation);
                }
            }

            return closestFloors;
        }

        /// There is always at least one element returned if no filter is used
        protected virtual List<Elevator> GetClosestElevators(Floor floor, Predicate<Elevator>? filter = null)
        {
            filter = filter ?? new Predicate<Elevator>(e => true);

            int minDistance = int.MaxValue;
            List<Elevator> closestElevators = new();

            foreach(Elevator elevator in Elevators.Where(e => filter(e)))
            {
                int distance = Math.Abs((elevator.Location - floor.Location).Value);

                if(distance < minDistance)
                {
                    minDistance = distance; 
                    closestElevators.Clear();
                    closestElevators.Add(elevator);
                }
                else if(distance == minDistance)
                {
                    closestElevators.Add(elevator);
                }
            }

            return closestElevators;
        }

        protected virtual Floor? ToWhereIsPlanned(Elevator elevator, Predicate<Floor>? filter = null)
        {
            filter = filter ?? new Predicate<Floor>(f => true);

            foreach(Floor floor in Floors.Where(f => filter(f)))
            {
                foreach(Elevator plannedElevator in floor.PlannedElevators)
                {
                    if(elevator == plannedElevator)
                    {
                        return floor;
                    }
                }
            }

            return null;
        }
    }

    public class StateDecisionTreeConditionNode
    {
        public StateDecisionTreeConditionNode OnTrue { get; }
        public StateDecisionTreeConditionNode OnFalse { get; }

        public Func<bool> Condition { get; set; }

        public StateDecisionTreeConditionNode(StateDecisionTreeConditionNode onTrue, StateDecisionTreeConditionNode onFalse, Func<bool> condition)
        {
            OnTrue = onTrue;
            OnFalse = onFalse;
            Condition = condition;
        }
    }

    public class StateDecisionTreeActionNode
    {
        public Action Action { get; set; }

        public StateDecisionTreeActionNode(Action action)
        {
            Action = action;
        }
    }
}
