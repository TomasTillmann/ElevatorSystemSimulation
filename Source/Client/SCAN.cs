using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;

namespace Client
{
    public class SCAN : ElevatorLogic<BasicRequest> 
    {
        protected ConditionAfterElevatorEvent<SCAN> StateDecisionTreeAfterElevatorEvent { get; private set; }
        protected ConditionAfterRequestEvent<SCAN, BasicRequest> StateDecisionTreeAfterRequestEvent { get; private set; }
        public SCAN(Building building)
        :base(building)
        {
            StateDecisionTreeAfterElevatorEvent = GetStateDecisionTreeAfterElevatorEvent();
            StateDecisionTreeAfterRequestEvent = GetStateDecisionTreeAfterRequestEvent();
        }

        public override void Execute(ISimulationState<BasicRequest> state)
        {
            StateDecisionTreeAfterRequestEvent.Execute(state);
        }

        public override void Execute(ISimulationState<ElevatorEvent> state)
        {
            // avoids cycling the elevator in constant idle action planning
            if(state.Event.FinishedAction == ElevatorSystemSimulation.ElevatorAction.Idle)
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
                    state.Event.EventLocation.Requests.Count != 0 ||
                    state.Event.Elevator.AttendingRequests.Any(r => r.Destination == state.Event.EventLocation);
            });

            ElevatorCondition arePeopleInElevator = new(this, (state,context) =>
            {
                return state.Event.Elevator.AttendingRequests.Count != 0;
            });

            shouldLoadUnload.OnTrue = new ElevatorAction(this, (state, context) =>
            {
                state.Event.Elevator.UnloadAndLoad(state.Event.EventLocation);

                return true;
            });

            shouldLoadUnload.OnFalse = arePeopleInElevator;

            arePeopleInElevator.OnTrue = new ParametrizedCondition<bool>(this, true, AreRequestsOutInSpecifiedDirection)
            {
                OnTrue = new ParametrizedCondition<bool>(this, true, AreRequestsInSpecifiedDirection)
                {
                    OnTrue = new ElevatorAction(this, (state, context) =>
                    {
                        Elevator elevator = state.Event.Elevator;

                        HashSet<Floor> floorsForRequestIn = new(GetClosestFloorsWithRequest(elevator,
                            floorFilter: GetFloorsFilterBasedOnDirection(true, elevator)));

                        HashSet<Floor> floorsForRequestOut = new(GetClosestFloorsWithRequestOut(elevator,
                            requestFilter: GetAttendingRequestsFilterBasedOnDirection(true, elevator)));

                        floorsForRequestIn.UnionWith(floorsForRequestOut);
                        List<Floor> floors = GetClosestFloors(state.Event.Elevator, floorsForRequestIn.ToList());

                        // There must be at least one floor, because of its position in the state decision tree
                        elevator.MoveTo(floors.First());

                        return true;
                    }),

                    OnFalse = new ElevatorAction(this, (state, context) =>
                    {
                        Elevator elevator = state.Event.Elevator;

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
                        Elevator elevator = state.Event.Elevator;

                        List<Floor> floors = GetClosestFloorsWithRequest(elevator,
                            floorFilter: GetFloorsFilterBasedOnDirection(true, elevator));

                        elevator.MoveTo(floors.First());

                        return true;
                    }),

                    OnFalse = new ParametrizedCondition<bool>(this, false, AreRequestsInSpecifiedDirection)
                    {
                        OnTrue = new ElevatorAction(this, (state, context) =>
                        {
                            Elevator elevator = state.Event.Elevator;

                            HashSet<Floor> floorsForRequestIn = new(GetClosestFloorsWithRequest(elevator,
                                floorFilter: GetFloorsFilterBasedOnDirection(false, elevator)));

                            HashSet<Floor> floorsForRequestOut = new(GetClosestFloorsWithRequestOut(elevator,
                                requestFilter: GetAttendingRequestsFilterBasedOnDirection(false, elevator)));

                            floorsForRequestIn.UnionWith(floorsForRequestOut);
                            List<Floor> floors = GetClosestFloors(state.Event.Elevator, floorsForRequestIn.ToList());

                            elevator.MoveTo(floors.First());

                            return true;
                        }),

                        OnFalse = new ElevatorAction(this, (state, context) =>
                        {
                            Elevator elevator = state.Event.Elevator;

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
                    Elevator elevator = state.Event.Elevator;

                    List<Floor> floors = GetClosestFloorsWithRequest(elevator,
                        floorFilter: GetFloorsFilterBasedOnDirection(true, elevator));

                    elevator.MoveTo(floors.First());

                    return true;
                }),

                OnFalse = new ParametrizedCondition<bool>(this, false, AreRequestsInSpecifiedDirection)
                {
                    OnTrue = new ElevatorAction(this, (state, context) =>
                    {
                        Elevator elevator = state.Event.Elevator;

                        List<Floor> floors = GetClosestFloorsWithRequest(elevator,
                            floorFilter: GetFloorsFilterBasedOnDirection(false, elevator));

                        elevator.MoveTo(floors.First());

                        return true;
                    }),

                    OnFalse = new ElevatorAction(this, (state,  context) =>
                    {
                        state.Event.Elevator.Idle(state.Event.EventLocation);

                        return true;
                    })
                }
            };

            return shouldLoadUnload;
        }

        private ConditionAfterRequestEvent<SCAN, BasicRequest> GetStateDecisionTreeAfterRequestEvent()
        {

            return new RequestCondition(this, (state,  context) =>
            {
                return context.Elevators.Any(e => e.IsIdle);
            })
            {
                OnTrue = new ElevatorActionAfterRequest(this, (state, context) =>
                {
                    GetClosestElevators(state.Event.EventLocation, filter : e => e.IsIdle)
                    .First().MoveTo(state.Event.EventLocation);

                    return true;
                }),

                OnFalse = null
            };
        }

        #region States

        #region AfterElevatorStates

        private class ElevatorCondition : ConditionAfterElevatorEvent<SCAN>
        {
            public Func<ISimulationState<ElevatorEvent>, SCAN, bool> InternalPredicate { get; set; }

            public ElevatorCondition(SCAN context, Func<ISimulationState<ElevatorEvent>, SCAN, bool> internalPredicate) : base(context)
            {
                InternalPredicate = internalPredicate;
            }

            protected override bool Predicate(ISimulationState<ElevatorEvent> state)
            {
                return InternalPredicate.Invoke(state, Context);
            }
        }

        private class ParametrizedCondition<ParameterType> : ConditionAfterElevatorEvent<SCAN>
        {
            public ParameterType Parameter { get; set; }
            public Func<ISimulationState<ElevatorEvent>, SCAN, ParameterType, bool> InternalPredicate { get; set; }

            public ParametrizedCondition(SCAN context, ParameterType memory, Func<ISimulationState<ElevatorEvent>, SCAN, ParameterType, bool> internalPredicate) : base(context)
            {
                InternalPredicate = internalPredicate;
                Parameter = memory;
            }

            protected override bool Predicate(ISimulationState<ElevatorEvent> state)
            {
                return InternalPredicate.Invoke(state, Context, Parameter);
            }
        }

        private class ElevatorAction : ActionAfterElevatorEvent<SCAN>
        {
            public Func<ISimulationState<ElevatorEvent>, SCAN, bool> InternalExecute { get; set; }

            public ElevatorAction(SCAN context, Func<ISimulationState<ElevatorEvent>, SCAN, bool> internalExecute) : base(context)
            {
                InternalExecute = internalExecute;
            }

            public override bool Execute(ISimulationState<ElevatorEvent> state)
            {
                return InternalExecute.Invoke(state, Context);
            }
        }

        #endregion

        #region AfterRequestStates

        private class RequestCondition : ConditionAfterRequestEvent<SCAN, BasicRequest>
        {
            public Func<ISimulationState<BasicRequest>, SCAN, bool> InternalPredicate { get; set; }

            public RequestCondition(SCAN context, Func<ISimulationState<BasicRequest>, SCAN, bool> internalPredicate) : base(context)
            {
                InternalPredicate = internalPredicate;
            }

            protected override bool Predicate(ISimulationState<BasicRequest> state)
            {
                return InternalPredicate.Invoke(state, Context);
            }
        }

        private class ElevatorActionAfterRequest : ActionAfterRequestEvent<SCAN, BasicRequest>
        {
            public Func<ISimulationState<BasicRequest>, SCAN, bool> InternalExecute { get; set; }

            public ElevatorActionAfterRequest(SCAN context, Func<ISimulationState<BasicRequest>, SCAN, bool> internalExecute) : base(context)
            {
                InternalExecute = internalExecute;
            }

            public override bool Execute(ISimulationState<BasicRequest> state)
            {
                return InternalExecute.Invoke(state, Context);
            }
        }

        #endregion

        #endregion

        #region HelpingMehotds

        private bool AreRequestsInSpecifiedDirection(ISimulationState<ElevatorEvent> state, SCAN context, bool inSameDirection)
        {
            Elevator elevator = state.Event.Elevator;
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

        private bool AreRequestsOutInSpecifiedDirection(ISimulationState<ElevatorEvent> state, SCAN context, bool inSameDirection)
        {
            Elevator elevator = state.Event.Elevator;
            Predicate<BasicRequest> filter = GetAttendingRequestsFilterBasedOnDirection(inSameDirection, elevator); 

            return elevator.AttendingRequests.Any(r => filter((BasicRequest)r));
        }

        private List<Floor> GetFloorsWithMaxWaitingTimeOnRequest(List<Floor>? floors = null)
        {
            floors = floors ?? Floors;

            Seconds maxWaitingTime = 0.ToSeconds();
            List<Floor> floorsWithMaxWaitingTimeOnRequest = new();
            foreach(Floor floor in floors)
            {
                foreach(BasicRequest request in floor.Requests)
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

        private Predicate<BasicRequest> GetAttendingRequestsFilterBasedOnDirection(bool inSameDirection, Elevator elevator)
        {
            Predicate<BasicRequest> filter = f => true;
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