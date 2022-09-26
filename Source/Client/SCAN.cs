using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;

namespace Client
{
    public class SCAN : ElevatorLogic<BasicRequestEvent> 
    {
        protected ConditionAfterElevatorEvent<SCAN> StateDecisionTreeAfterElevatorEvent { get; private set; }
        protected ConditionAfterRequestEvent<SCAN, BasicRequestEvent> StateDecisionTreeAfterRequestEvent { get; private set; }
        public SCAN(Building building)
        :base(building)
        {
            StateDecisionTreeAfterElevatorEvent = GetStateDecisionTreeAfterElevatorEvent();
            StateDecisionTreeAfterRequestEvent = GetStateDecisionTreeAfterRequestEvent();
        }

        protected override void Execute(SimulationState<BasicRequestEvent> state)
        {
            StateDecisionTreeAfterRequestEvent.Execute(state);
        }

        protected override void Execute(SimulationState<ElevatorEvent> state)
        {
            // avoids cycling the elevator in constant idle action planning
            if(state.CurrentEvent.FinishedAction == ElevatorSystemSimulation.ElevatorAction.Idle)
            {
                return;
            }

            StateDecisionTreeAfterElevatorEvent.Execute(state);
        }

        private ConditionAfterElevatorEvent<SCAN> GetStateDecisionTreeAfterElevatorEvent()
        {
            ElevatorCondition shouldLoadUnload = new(this, (state,context) =>
            {
                return 
                    state.CurrentEvent.EventLocation.Requests.Count != 0 ||
                    state.CurrentEvent.Elevator.AttendingRequests.Any(r => r.Destination == state.CurrentEvent.EventLocation);
            });

            ElevatorCondition arePeopleInElevator = new(this, (state,context) =>
            {
                return state.CurrentEvent.Elevator.AttendingRequests.Count != 0;
            });

            shouldLoadUnload.OnTrue = new ElevatorAction(this, (state, context) =>
            {
                state.CurrentEvent.Elevator.UnloadAndLoad(state.CurrentEvent.EventLocation);

                return true;
            });

            shouldLoadUnload.OnFalse = arePeopleInElevator;

            arePeopleInElevator.OnTrue = new ParametrizedCondition<bool>(this, true, AreRequestsOutInSpecifiedDirection)
            {
                OnTrue = new ParametrizedCondition<bool>(this, true, AreRequestsInSpecifiedDirection)
                {
                    OnTrue = new ElevatorAction(this, (state, context) =>
                    {
                        Elevator elevator = state.CurrentEvent.Elevator;

                        HashSet<Floor> floorsForRequestIn = new(GetClosestFloorsWithRequest(elevator,
                            floorFilter: GetFloorsFilterBasedOnDirection(true, elevator)));

                        HashSet<Floor> floorsForRequestOut = new(GetClosestFloorsWithRequestOut(elevator,
                            requestFilter: GetAttendingRequestsFilterBasedOnDirection(true, elevator)));

                        floorsForRequestIn.UnionWith(floorsForRequestOut);
                        List<Floor> floors = GetClosestFloors(state.CurrentEvent.Elevator, floorsForRequestIn.ToList());

                        // There must be at least one floor, because of its position in the state decision tree
                        elevator.MoveTo(floors.First());

                        return true;
                    }),

                    OnFalse = new ElevatorAction(this, (state, context) =>
                    {
                        Elevator elevator = state.CurrentEvent.Elevator;

                        List<Floor> floors = GetClosestFloorsWithRequestOut(elevator,
                            requestFilter: GetAttendingRequestsFilterBasedOnDirection(true, elevator));

                        elevator.MoveTo(floors.First());

                        return true;
                    }) 
                },

                OnFalse = new ParametrizedCondition<bool>(this, true, AreRequestsInSpecifiedDirection)
                {
                    OnTrue = new ElevatorAction(this, (state, context) =>
                    {
                        Elevator elevator = state.CurrentEvent.Elevator;

                        List<Floor> floors = GetClosestFloorsWithRequest(elevator,
                            floorFilter: GetFloorsFilterBasedOnDirection(true, elevator));

                        elevator.MoveTo(floors.First());

                        return true;
                    }),

                    OnFalse = new ParametrizedCondition<bool>(this, false, AreRequestsInSpecifiedDirection)
                    {
                        OnTrue = new ElevatorAction(this, (state, context) =>
                        {
                            Elevator elevator = state.CurrentEvent.Elevator;

                            HashSet<Floor> floorsForRequestIn = new(GetClosestFloorsWithRequest(elevator,
                                floorFilter: GetFloorsFilterBasedOnDirection(false, elevator)));

                            HashSet<Floor> floorsForRequestOut = new(GetClosestFloorsWithRequestOut(elevator,
                                requestFilter: GetAttendingRequestsFilterBasedOnDirection(false, elevator)));

                            floorsForRequestIn.UnionWith(floorsForRequestOut);
                            List<Floor> floors = GetClosestFloors(state.CurrentEvent.Elevator, floorsForRequestIn.ToList());

                            elevator.MoveTo(floors.First());

                            return true;
                        }),

                        OnFalse = new ElevatorAction(this, (state, context) =>
                        {
                            Elevator elevator = state.CurrentEvent.Elevator;

                            List<Floor> floors = GetClosestFloorsWithRequestOut(elevator,
                            requestFilter: GetAttendingRequestsFilterBasedOnDirection(false, elevator));

                            elevator.MoveTo(floors.First());

                            return true;
                        })
                    }
                }
            };

            arePeopleInElevator.OnFalse = new ParametrizedCondition<bool>(this, true, AreRequestsInSpecifiedDirection)
            {
                OnTrue = new ElevatorAction(this, (state, context) =>
                {
                    Elevator elevator = state.CurrentEvent.Elevator;

                    List<Floor> floors = GetClosestFloorsWithRequest(elevator,
                        floorFilter: GetFloorsFilterBasedOnDirection(true, elevator));

                    elevator.MoveTo(floors.First());

                    return true;
                }),

                OnFalse = new ParametrizedCondition<bool>(this, false, AreRequestsInSpecifiedDirection)
                {
                    OnTrue = new ElevatorAction(this, (state, context) =>
                    {
                        Elevator elevator = state.CurrentEvent.Elevator;

                        List<Floor> floors = GetClosestFloorsWithRequest(elevator,
                            floorFilter: GetFloorsFilterBasedOnDirection(false, elevator));

                        elevator.MoveTo(floors.First());

                        return true;
                    }),

                    OnFalse = new ElevatorAction(this, (state,  context) =>
                    {
                        state.CurrentEvent.Elevator.Idle(state.CurrentEvent.EventLocation);

                        return true;
                    })
                }
            };

            return shouldLoadUnload;
        }

        private ConditionAfterRequestEvent<SCAN, BasicRequestEvent> GetStateDecisionTreeAfterRequestEvent()
        {

            return new RequestCondition(this, (state,  context) =>
            {
                return context.Elevators.Any(e => e.IsIdle);
            })
            {
                OnTrue = new ElevatorActionAfterRequest(this, (state, context) =>
                {
                    GetClosestElevators(state.CurrentEvent.EventLocation).First().MoveTo(state.CurrentEvent.EventLocation);

                    return true;
                }),

                OnFalse = null
            };
        }

        #region States

        #region AfterElevatorStates

        private class ElevatorCondition : ConditionAfterElevatorEvent<SCAN>
        {
            public Func<SimulationState<ElevatorEvent>, SCAN, bool> InternalPredicate { get; set; }

            public ElevatorCondition(SCAN context, Func<SimulationState<ElevatorEvent>, SCAN, bool> internalPredicate) : base(context)
            {
                InternalPredicate = internalPredicate;
            }

            protected override bool Predicate(SimulationState<ElevatorEvent> state)
            {
                return InternalPredicate.Invoke(state, Context);
            }
        }

        private class ParametrizedCondition<ParameterType> : ConditionAfterElevatorEvent<SCAN>
        {
            public ParameterType Parameter { get; set; }
            public Func<SimulationState<ElevatorEvent>, SCAN, ParameterType, bool> InternalPredicate { get; set; }

            public ParametrizedCondition(SCAN context, ParameterType memory, Func<SimulationState<ElevatorEvent>, SCAN, ParameterType, bool> internalPredicate) : base(context)
            {
                InternalPredicate = internalPredicate;
                Parameter = memory;
            }

            protected override bool Predicate(SimulationState<ElevatorEvent> state)
            {
                return InternalPredicate.Invoke(state, Context, Parameter);
            }
        }

        private class ElevatorAction : ActionAfterElevatorEvent<SCAN>
        {
            public Func<SimulationState<ElevatorEvent>, SCAN, bool> InternalExecute { get; set; }

            public ElevatorAction(SCAN context, Func<SimulationState<ElevatorEvent>, SCAN, bool> internalExecute) : base(context)
            {
                InternalExecute = internalExecute;
            }

            public override bool Execute(SimulationState<ElevatorEvent> state)
            {
                return InternalExecute.Invoke(state, Context);
            }
        }

        #endregion

        #region AfterRequestStates

        private class RequestCondition : ConditionAfterRequestEvent<SCAN, BasicRequestEvent>
        {
            public Func<SimulationState<BasicRequestEvent>, SCAN, bool> InternalPredicate { get; set; }

            public RequestCondition(SCAN context, Func<SimulationState<BasicRequestEvent>, SCAN, bool> internalPredicate) : base(context)
            {
                InternalPredicate = internalPredicate;
            }

            protected override bool Predicate(SimulationState<BasicRequestEvent> state)
            {
                return InternalPredicate.Invoke(state, Context);
            }
        }

        private class ElevatorActionAfterRequest : ActionAfterRequestEvent<SCAN, BasicRequestEvent>
        {
            public Func<SimulationState<BasicRequestEvent>, SCAN, bool> InternalExecute { get; set; }

            public ElevatorActionAfterRequest(SCAN context, Func<SimulationState<BasicRequestEvent>, SCAN, bool> internalExecute) : base(context)
            {
                InternalExecute = internalExecute;
            }

            public override bool Execute(SimulationState<BasicRequestEvent> state)
            {
                return InternalExecute.Invoke(state, Context);
            }
        }

        #endregion

        #endregion

        #region HelpingMehotds

        private bool AreRequestsInSpecifiedDirection(SimulationState<ElevatorEvent> state, SCAN context, bool inSameDirection)
        {
            Elevator elevator = state.CurrentEvent.Elevator;
            Predicate<Floor> filter = GetFloorsFilterBasedOnDirection(inSameDirection, elevator);

            foreach (Floor floor in context.Floors.Where(f => filter(f)))
            {
                if (floor.Requests.Count != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool AreRequestsOutInSpecifiedDirection(SimulationState<ElevatorEvent> state, SCAN context, bool inSameDirection)
        {
            Elevator elevator = state.CurrentEvent.Elevator;
            Predicate<BasicRequestEvent> filter = GetAttendingRequestsFilterBasedOnDirection(inSameDirection, elevator); 

            return elevator.AttendingRequests.Any(r => filter((BasicRequestEvent)r));
        }

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

        private List<Floor> GetClosestFloors(Elevator elevator, List<Floor>? floors = null)
        {
            floors = floors ?? Floors;
            Centimeters minDistance = new(int.MaxValue);
            List<Floor> closestFloors = new();

            foreach (Floor floor in floors)
            {
                if (GetDistance(elevator, floor) < minDistance)
                {
                    minDistance = GetDistance(elevator, floor); 
                    closestFloors.Clear();
                    closestFloors.Add(floor);
                }
                else if (GetDistance(elevator, floor) == minDistance)
                {
                    closestFloors.Add(floor);
                }
            }

            return closestFloors;
        }

        private Predicate<BasicRequestEvent> GetAttendingRequestsFilterBasedOnDirection(bool inSameDirection, Elevator elevator)
        {
            Predicate<BasicRequestEvent> filter = f => true;
            Direction? fromWhereElevatorGotThere = elevator.LastDirection;

            if ((fromWhereElevatorGotThere == Direction.Up && inSameDirection) ||
               (fromWhereElevatorGotThere == Direction.Down && !inSameDirection))
            {
                filter = r => r.Destination.Location >= elevator.Location;
            }
            else if ((fromWhereElevatorGotThere == Direction.Down && inSameDirection) ||
                    (fromWhereElevatorGotThere == Direction.Up && !inSameDirection))
            {
                filter = r => r.Destination.Location <= elevator.Location;
            }
            // else direction is null or no direction, all floors are "in the same direction" - no filter is applied

            return filter;
        }

        private Predicate<Floor> GetFloorsFilterBasedOnDirection(bool inSameDirection, Elevator elevator)
        {
            Direction? fromWhereElevatorGotThere = elevator.LastDirection;

            Predicate<Floor> filter = f => true;

            if ((fromWhereElevatorGotThere == Direction.Up && inSameDirection) ||
               (fromWhereElevatorGotThere == Direction.Down && !inSameDirection))
            {
                filter = f => f.Location >= elevator.Location;
            }
            else if ((fromWhereElevatorGotThere == Direction.Down && inSameDirection) ||
                    (fromWhereElevatorGotThere == Direction.Up && !inSameDirection))
            {
                filter = f => f.Location <= elevator.Location;
            }
            // else direction is null or no direction, all floors are "in the same direction" - no filter is applied

            return filter;
        }

        #endregion
    }

}