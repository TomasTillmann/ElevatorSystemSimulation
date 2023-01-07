using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public abstract class ElevatorLogic<TRequestEvent> : IElevatorLogic<TRequestEvent> where TRequestEvent : Request
    {
        #region Context

        public Building Building { get; }
        protected List<Elevator> Elevators => Building.ElevatorSystem.Elevators;
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

            if(state.CurrentEvent is TRequestEvent ce)
            {
                ce.EventLocation._Requests.Add(ce);

                Execute(new SimulationState<TRequestEvent>(ce, state.CurrentTime));
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

        public abstract void Execute(ISimulationState<TRequestEvent> requestEvent);
        public abstract void Execute(ISimulationState<ElevatorEvent> elevatorEvent);


        #region Helpers

        protected Centimeters GetDistance(ILocatable l1, ILocatable l2)
        {
            return Math.Abs((l1.Location - l2.Location).Value).ToCentimeters();
        }

        protected virtual IEnumerable<TRequestEvent> GetAllCurrentRequestEvents(Predicate<Floor>? filter = null)
        {
            filter = filter ?? new Predicate<Floor>(f => true);

            foreach(Floor floor in Building.Floors.Value.Where(f => filter(f)))
            {
                foreach(TRequestEvent requestEvent in floor.Requests)
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

            foreach (TRequestEvent request in GetAllCurrentRequestEvents())
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

        protected virtual List<Floor> GetClosestFloorsWithRequestOut(Elevator elevator, Predicate<TRequestEvent>? requestFilter = null)
        {
            requestFilter = requestFilter ?? new Predicate<TRequestEvent>(r => true);

            Centimeters minDistance = new(int.MaxValue);
            List<Floor> closestFloors = new();

            foreach(TRequestEvent request in elevator.AttendingRequests.Where(r => requestFilter((TRequestEvent)r)))
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

    public abstract class ConditionAfterElevatorEvent<TContextType> : StateDecisionTreeConditionNode<ISimulationState<ElevatorEvent>, TContextType> where TContextType : IElevatorLogic
    {
        protected ConditionAfterElevatorEvent(
            IStateDecisionTreeNode<ISimulationState<ElevatorEvent>, TContextType> onTrue,
            IStateDecisionTreeNode<ISimulationState<ElevatorEvent>, TContextType> onFalse,
            TContextType context
        )
        : base(onTrue, onFalse, context) { }

        protected ConditionAfterElevatorEvent(TContextType context) : base(context) { }
    }

    public abstract class ActionAfterElevatorEvent<TContextType> : StateDecisionTreeActionNode<ISimulationState<ElevatorEvent>, TContextType> where TContextType : IElevatorLogic
    {
        protected ActionAfterElevatorEvent(TContextType context) : base(context) { }
    }

    public abstract class ConditionAfterRequestEvent<TContextType, TRequestType> : StateDecisionTreeConditionNode<ISimulationState<TRequestType>, TContextType> where TContextType : IElevatorLogic where TRequestType : Interfaces.Request 
    {
        protected ConditionAfterRequestEvent(
            IStateDecisionTreeNode<ISimulationState<TRequestType>, TContextType> onTrue,
            IStateDecisionTreeNode<ISimulationState<TRequestType>, TContextType> onFalse,
            TContextType context
        )
        : base(onTrue, onFalse, context) { }

        protected ConditionAfterRequestEvent(TContextType context) : base(context) { }
    }

    public abstract class ActionAfterRequestEvent<TContextType, TRequestType> : StateDecisionTreeActionNode<ISimulationState<TRequestType>, TContextType> where TContextType : IElevatorLogic where TRequestType : Interfaces.Request
    {
        protected ActionAfterRequestEvent(TContextType context) : base(context) { }
    }
}
