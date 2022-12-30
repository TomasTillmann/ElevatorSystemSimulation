using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Hungry : ElevatorLogic<BasicRequestEvent>
    {
        public Hungry(Building building) : base(building) { }

        protected override void Execute(SimulationState<BasicRequestEvent> state)
        {
            IEnumerable<Elevator> elevators;
            Elevator elevator;

            // find idle
            elevators = Building.ElevatorSystem.Elevators.Where(e => e.IsIdle);

            if (elevators.Any())
            {
                // find the closest idle
                elevator = elevators.FindMinSubset(e => Math.Abs((e.Location - state.CurrentEvent.Location).Value), int.MaxValue).First();
                elevator.MoveTo(state.CurrentEvent.EventLocation);
                return;
            }

            // find the closest generally
            elevators = Building.ElevatorSystem.Elevators.FindMinSubset(e => Math.Abs((e.Location - state.CurrentEvent.Location).Value), int.MaxValue);

            elevator = elevators.First();
            elevator.MoveTo(state.CurrentEvent.EventLocation);
        }

        /// <summary>
        /// Loads and unloads the elevator if needed. Plans the elevator to the closest floor with request.
        /// </summary>
        /// <param name="state"></param>
        protected override void Execute(SimulationState<ElevatorEvent> state)
        {
            Elevator elevator = state.CurrentEvent.Elevator;

            switch (state.CurrentEvent.FinishedAction)
            {
                case ElevatorAction.MoveTo:
                    // if any requests on the floor or people in the elevator want to get out on this floor 
                    if(state.CurrentEvent.EventLocation.Requests.Any()
                    || elevator.AttendingRequests.Any(r => r.Destination == state.CurrentEvent.EventLocation))
                    {
                        elevator.UnloadAndLoad(state.CurrentEvent.EventLocation);
                    }
                    else
                    {
                        Floor? floor = GetClosestRequestInFloor(state);
                        elevator.MoveTo(floor != null ? floor : state.CurrentEvent.EventLocation);
                    }

                    break;

                case ElevatorAction.UnloadAndLoad:
                    if (elevator.AttendingRequests.Any())
                    {
                        Floor floor = GetClosestRequestOutFloor(elevator);
                        elevator.MoveTo(floor);
                    }
                    else
                    {
                        Floor? floor = GetClosestRequestInFloor(state);
                        elevator.MoveTo(floor != null ? floor : state.CurrentEvent.EventLocation);
                    }

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
        private Floor? GetClosestRequestInFloor(SimulationState<ElevatorEvent> state)
        {
            IEnumerable<Floor> floorsWithRequest   = Building.Floors.Value.Where(f => f.Requests.Any());
            IEnumerable<Floor> closestFloors       = floorsWithRequest.FindMinSubset(f => Math.Abs((f.Location - state.CurrentEvent.Location).Value), int.MaxValue);

            return closestFloors.FirstOrDefault();
        }

        private Floor GetClosestRequestOutFloor(Elevator elevator)
        {
            return elevator.AttendingRequests.FindMinSubset(r => Math.Abs((r.Destination.Location - elevator.Location).Value), int.MaxValue).First().Destination;
        }
    }
}
