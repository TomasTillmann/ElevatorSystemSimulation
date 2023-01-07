using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Hungry : ElevatorLogic<BasicRequest>
    {
        public Hungry(Building building) : base(building) { }

        public override void Execute(ISimulationState<BasicRequest> state)
        {
            IEnumerable<Elevator> elevators;
            Elevator elevator;

            // find idle
            elevators = Building.ElevatorSystem.Elevators.Where(e => e.IsIdle);

            if (elevators.Any())
            {
                // find the closest idle
                elevator = elevators.FindMinSubset(e => Math.Abs((e.Location - state.Event.Location).Value), int.MaxValue).First();
                elevator.MoveTo(state.Event.EventLocation);
                return;
            }

            // find the closest generally
            elevators = Building.ElevatorSystem.Elevators.FindMinSubset(e => Math.Abs((e.Location - state.Event.Location).Value), int.MaxValue);

            elevator = elevators.First();
            elevator.MoveTo(state.Event.EventLocation);
        }

        /// <summary>
        /// Loads and unloads the elevator if needed. Plans the elevator to the closest floor with request.
        /// </summary>
        /// <param name="state"></param>
        public override void Execute(ISimulationState<ElevatorEvent> state)
        {
            Elevator elevator = state.Event.Elevator;

            switch (state.Event.FinishedAction)
            {
                case ElevatorAction.MoveTo:
                    // if any requests on the floor or people in the elevator want to get out on this floor 
                    if(state.Event.EventLocation.Requests.Any()
                    || elevator.AttendingRequests.Any(r => r.Destination == state.Event.EventLocation))
                    {
                        elevator.Unload(state.Event.EventLocation);
                    }
                    else
                    {
                        Floor? floor = GetClosestRequestInFloor(state);
                        elevator.MoveTo(floor != null ? floor : state.Event.EventLocation);
                    }

                    break;

                case ElevatorAction.Load:
                    if (elevator.AttendingRequests.Any())
                    {
                        Floor floor = GetClosestRequestOutFloor(elevator);
                        elevator.MoveTo(floor);
                    }
                    else
                    {
                        Floor? floor = GetClosestRequestInFloor(state);
                        elevator.MoveTo(floor != null ? floor : state.Event.EventLocation);
                    }

                    break;

                case ElevatorAction.Unload:
                    elevator.Load(state.Event.EventLocation);

                    break;

                case ElevatorAction.Idle:
                    return;
            }
        }

        /// <summary>
        /// Gets the floor with closest request, not caring about current direction (as in SCAN for example)
        /// </summary>
        /// <param name="state"></param>
        /// <returns>Closest floor with request or null. Null if there are no floors with any request.</returns>
        private Floor? GetClosestRequestInFloor(ISimulationState<ElevatorEvent> state)
        {
            IEnumerable<Floor> floorsWithRequest   = Building.Floors.Value.Where(f => f.Requests.Any());
            IEnumerable<Floor> closestFloors       = floorsWithRequest.FindMinSubset(f => Math.Abs((f.Location - state.Event.Location).Value), int.MaxValue);

            return closestFloors.FirstOrDefault();
        }

        private Floor GetClosestRequestOutFloor(Elevator elevator)
        {
            return elevator.AttendingRequests.FindMinSubset(r => Math.Abs((r.Destination.Location - elevator.Location).Value), int.MaxValue).First().Destination;
        }
    }
}
