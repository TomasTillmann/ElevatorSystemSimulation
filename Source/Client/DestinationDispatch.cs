using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class DestinationDispatch : ElevatorLogic<BasicRequest>
    {
        private Dictionary<Elevator, DispatchElevator> DispatchElevators = new();
        private HashSet<Floor> FreeGroups = new();

        public DestinationDispatch(Building building) : base(building)
        {
            foreach(Elevator elevator in building.ElevatorSystem.Elevators)
            {
                DispatchElevators.Add(elevator, new DispatchElevator(elevator));
            }

            foreach(Floor floor in building.Floors.Value)
            {
                FreeGroups.Add(floor);
            }
        } 

        public override void Execute(ISimulationState<BasicRequest> state)
        {
            HashSet<DispatchElevator> elevatorCandidates = new();
            Floor currentFloor = state.Event.EventLocation;

            if (FreeGroups.Contains(state.Event.Destination))
            {
                // add all idle
                foreach(Elevator elevator in Building.ElevatorSystem.Elevators.Where(e => e.IsIdle))
                {
                    elevatorCandidates.Add(DispatchElevators[elevator]);
                }

                // add without any group
                foreach(Elevator elevator in Building.ElevatorSystem.Elevators.Where(e => DispatchElevators[e].Group is null))
                {
                    elevatorCandidates.Add(DispatchElevators[elevator]);
                }
            }
            else
            {
                // add only those dealing with the same group
                foreach(Elevator elevator in Building.ElevatorSystem.Elevators.Where(e => DispatchElevators[e].Group == state.Event.Destination))
                {
                    elevatorCandidates.Add(DispatchElevators[elevator]);
                }
            }

            // elevators in same direction and with no directoin
            IEnumerable<DispatchElevator> elevatorsInSameDir = elevatorCandidates 
                .Where(e => (currentFloor.Location - e.Value.Location).Value * (int)e.Value.Direction >= 0);

            // Only scenerio with this true is elevators dealing with the same group are going in the opposite direction.
            if (!elevatorsInSameDir.Any())
            {
                return;
            }

            IEnumerable<DispatchElevator> closestElevators = elevatorsInSameDir
                .Where(e => e.Value.PlannedTo is null || e.Value.Direction == Direction.NoDirection || Math.Abs((e.Value.PlannedTo.Location - e.Value.Location).Value) > Math.Abs((currentFloor.Location - e.Value.Location).Value))
                .FindMinSubset(e => Math.Abs(e.Value.Location.Value - currentFloor.Location.Value), int.MaxValue);

            DispatchElevator? bestElevator = closestElevators.FirstOrDefault(); 

            if(bestElevator is null)
            {
                return;
            }

            bestElevator.Value.MoveTo(currentFloor);
            bestElevator.Group = state.Event.Destination;
            FreeGroups.Remove(bestElevator.Group);
        }

        public override void Execute(ISimulationState<ElevatorEvent> state)
        {
            DispatchElevator elevator = DispatchElevators[state.Event.Elevator];
            Floor currentFloor = state.Event.EventLocation;

            switch (state.Event.FinishedAction)
            {
                case ElevatorAction.Load:
                    // elevator is full - ready to depart
                    if(elevator.Value.AttendingRequests.Count == elevator.Value.Capacity)
                    {
                        elevator.Value.MoveTo(elevator.Group);
                    }
                    else
                    {
                        Floor? destination = GetClosestFloorsInDirectionInGroup(elevator, elevator.Value.Direction).FirstOrDefault();

                        // no more people in the same group - ready to depart
                        if(destination is null)
                        {
                            elevator.Value.MoveTo(elevator.Group);
                        }
                        else
                        {
                            // move to the floor where request with the same group is
                            elevator.Value.MoveTo(destination);
                        }
                    }

                    break;

                case ElevatorAction.Unload:
                    FreeGroups.Add(elevator.Group!);

                    Floor? group = GetNewGroup();
                    if(group is null)
                    {
                        elevator.Value.Idle(currentFloor);
                    }
                    else
                    {
                        elevator.Group = group;
                        FreeGroups.Remove(group);

                        Floor? destination = GetClosestFloorsInDirectionInGroup(elevator, elevator.Value.Direction).FirstOrDefault();
                        if(destination is null)
                        {
                            elevator.Value.Idle(currentFloor);
                        }
                        else
                        {
                            elevator.Value.MoveTo(destination);
                        }
                    }

                    break;

                case ElevatorAction.MoveTo:
                    if(elevator.Group == currentFloor) 
                    {
                        elevator.Value.Unload(currentFloor);
                    }
                    else
                    {
                        elevator.Value.Load(currentFloor, currentFloor.Requests.Where(r => r.Destination == elevator.Group));
                    }

                    break;

                case ElevatorAction.Idle:
                    // there is request and no elevators are planned to handle this request (there still might not be planned to this specific request though)
                    IEnumerable<Floor> floorsWithRequest = Floors.Where(f => f.Requests.Any() && !f.PlannedElevators.Any());
                    Floor? floor = floorsWithRequest.FindMinSubset(f => Math.Abs((f.Location - elevator.Value.Location).Value), int.MaxValue).FirstOrDefault();

                    elevator.Value.MoveTo(floor);

                    break;
            }
        }

        private IEnumerable<Floor> GetClosestFloorsInDirectionInGroup(DispatchElevator elevator, Direction direction)
        {
            IEnumerable<Floor> floorsInSameDirectionInGroup = GetFloorsInDirection(elevator, direction).Where(f => f.Requests.Any(r => r.Destination == elevator.Group));
            return floorsInSameDirectionInGroup.FindMinSubset(f => Math.Abs((f.Location - elevator.Value.Location).Value), int.MaxValue);
        }

        private IEnumerable<Floor> GetFloorsInDirection(DispatchElevator elevator, Direction direction)
        {
            IEnumerable<Floor> floorsInSameDirection;

            if(direction == Direction.Up)
            {
                floorsInSameDirection = Building.Floors.Value.Where(f => f.Location >= elevator.Value.Location); 
            }
            else if(direction == Direction.Down)
            {
                floorsInSameDirection = Building.Floors.Value.Where(f => f.Location <= elevator.Value.Location); 
            }
            else
            {
                floorsInSameDirection = Building.Floors.Value;
            }

            return floorsInSameDirection;
        }

        private Floor? GetNewGroup()
        {
            Dictionary<Floor, int> groupsCount = new();
            Floors.ForEach(f => groupsCount.Add(f, 0));

            foreach(Floor floor in Floors)
            {
                foreach(BasicRequest request in floor.Requests)
                {
                    groupsCount[request.Destination] += 1;
                }
            }

            foreach(var entry in from entry in groupsCount orderby entry.Value descending select entry)
            {
                if (FreeGroups.Contains(entry.Key))
                {
                    FreeGroups.Remove(entry.Key);
                    return entry.Key;
                }
            }

            return null;
        }
    }

    public class DispatchElevator
    {
        public Floor? Group { get; set; }
        public Elevator Value { get; set; }


        public DispatchElevator(Elevator elevator)
        {
            Value = elevator;
        }
    }
}
