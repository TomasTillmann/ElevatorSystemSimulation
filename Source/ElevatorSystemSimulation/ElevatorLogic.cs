using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public abstract class ElevatorLogic<RequestEvent> : IElevatorLogic where RequestEvent : IRequestEvent
    {
        #region Context

        public Building Building { get; }
        protected List<Elevator> Elevators => Building.ElevatorSystem.Value;
        protected List<Floor> Floors => Building.Floors.Value;

        #endregion

        #region State

        protected Seconds CurrentTime { get; private set; }

        #endregion

        #region Helpers

        protected Dictionary<ElevatorAction, Action<ElevatorEvent>> _DoAfterElevatorAction { get; } = new();

        #endregion

        public ElevatorLogic(Building building)
        {
            Building = building;
        }

        /// Elevator logic decides what the best decision is - how to plan the elevators - based on the state
        public void Execute(ISimulationState state)
        {
            CurrentTime = state.CurrentTime;

            if(state.CurrentEvent is RequestEvent ce)
            {
                ce.EventLocation._Requests.Add(ce);

                Execute(new SimulationState<RequestEvent>(ce, state.CurrentTime));
            }
            else if(state.CurrentEvent is ElevatorEvent ee)
            {
                Execute(new SimulationState<ElevatorEvent>(ee, state.CurrentTime));
            }
            else
            {
                throw new Exception("Event is something different. And it shouldn't be.");
            }
        }

        protected abstract void Execute(SimulationState<RequestEvent> state);
        protected abstract void Execute(SimulationState<ElevatorEvent> state);

        #region Helpers

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

        protected virtual List<Floor> GetClosestFloorsWithRequest(Elevator elevator, Predicate<Floor>? floorFilter = null)
        {
            floorFilter = floorFilter ?? new Predicate<Floor>(f => true);

            int minDistance = int.MaxValue;
            List<Floor> closestFloors = new();

            foreach (RequestEvent request in GetAllCurrentRequestEvents())
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

        protected virtual List<Floor> GetClosestFloorsWithRequestOut(Elevator elevator, Predicate<RequestEvent>? requestFilter = null)
        {
            requestFilter = requestFilter ?? new Predicate<RequestEvent>(r => true);

            Centimeters minDistance = new(int.MaxValue);
            List<Floor> closestFloors = new();

            foreach(RequestEvent request in elevator.AttendingRequests.Where(r => requestFilter((RequestEvent)r)))
            {
                Centimeters distance = GetDistance(request.Destination, elevator);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestFloors.Clear();
                    closestFloors.Add(request.Destination);
                }
                else if(distance == minDistance)
                {
                    closestFloors.Add(request.Destination);
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

        private List<Floor> GetClosestFloorsFromSource()
        {
            //TODO - make intersection of all closest to methods - their body is almost the same
            throw new NotImplementedException();
        }

        #endregion

    }

    public abstract class ConditionAfterElevatorEvent<ContextType> : StateDecisionTreeConditionNode<SimulationState<ElevatorEvent>, ContextType> where ContextType : IElevatorLogic
    {
        protected ConditionAfterElevatorEvent(
            IStateDecisionTreeNode<SimulationState<ElevatorEvent>, ContextType> onTrue,
            IStateDecisionTreeNode<SimulationState<ElevatorEvent>, ContextType> onFalse,
            ContextType context
        )
        : base(onTrue, onFalse, context) { }

        protected ConditionAfterElevatorEvent(ContextType context) : base(context) { }
    }

    public abstract class ActionAfterElevatorEvent<ContextType> : StateDecisionTreeActionNode<SimulationState<ElevatorEvent>, ContextType> where ContextType : IElevatorLogic
    {
        protected ActionAfterElevatorEvent(ContextType context) : base(context) { }
    }

    public abstract class ConditionAfterRequestEvent<ContextType, RequestType> : StateDecisionTreeConditionNode<SimulationState<RequestType>, ContextType> where ContextType : IElevatorLogic where RequestType : IRequestEvent 
    {
        protected ConditionAfterRequestEvent(
            IStateDecisionTreeNode<SimulationState<RequestType>, ContextType> onTrue,
            IStateDecisionTreeNode<SimulationState<RequestType>, ContextType> onFalse,
            ContextType context
        )
        : base(onTrue, onFalse, context) { }

        protected ConditionAfterRequestEvent(ContextType context) : base(context) { }
    }

    public abstract class ActionAfterRequestEvent<ContextType, RequestType> : StateDecisionTreeActionNode<SimulationState<RequestType>, ContextType> where ContextType : IElevatorLogic where RequestType : IRequestEvent
    {
        protected ActionAfterRequestEvent(ContextType context) : base(context) { }
    }
}
