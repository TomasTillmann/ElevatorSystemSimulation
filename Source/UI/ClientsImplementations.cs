using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UI
{
    public class ClientsElevatorLogic : ElevatorLogic<ClientsRequestEvent>
    {
        private List<Elevator> _Elevators { get; set; }
        private Floors _Floors { get; set; }
        private Random _Random { get; }
        private readonly Dictionary<ElevatorAction, Action<ElevatorEvent>> _DoAfterElevatorAction = new();

        public ClientsElevatorLogic(Building building)
        : base(building)
        {
            _Elevators = building.ElevatorSystem.Elevators;
            _Floors = building.Floors;
            _Random = new Random();

            _DoAfterElevatorAction.Add(ElevatorAction.MoveTo, StepAfterMove);
            _DoAfterElevatorAction.Add(ElevatorAction.UnloadAndLoad, StepAfterUnloadAndLoad);
            _DoAfterElevatorAction.Add(ElevatorAction.Idle, StepAfterIdle);
        }

        protected override void Step(ClientsRequestEvent e)
        {
            Elevator? freeElevator = _Elevators.Find(elevator => elevator.IsIdle);

            if (freeElevator != null)
            {
                freeElevator.MoveTo(e.Floor);
            }
        }

        protected override void Step(ElevatorEvent e)
        {
            _DoAfterElevatorAction[e.FinishedAction].Invoke(e);
        }

        private void StepAfterMove(ElevatorEvent e)
        {
            if (e.Elevator.AttendingRequests.Count > 0 || e.CurrentFloor.Requests.Count > 0)
            {
                e.Elevator.UnloadAndLoad(e.CurrentFloor);
            }
            else if (GetAllCurrentRequestEvents().Any())
            {
                e.Elevator.MoveTo(GetClosestFloorWithRequest(e.Elevator));
            }
            else
            {
                e.Elevator.Idle(e.CurrentFloor);

                Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

                if (freeElevator != null)
                {
                    freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
                }
            }
        }

        private void StepAfterUnloadAndLoad(ElevatorEvent e)
        {
            if (GetAllCurrentRequestEvents().Any())
            {
                Floor floor = GetClosestFloorWithRequest(e.Elevator);
                e.Elevator.MoveTo(floor);
            }
            else
            {
                e.Elevator.Idle(e.CurrentFloor);

                Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

                if (freeElevator != null)
                {
                    freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
                }
            }
        }

        private void StepAfterIdle(ElevatorEvent e)
        {
            Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

            if (freeElevator != null)
            {
                freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
            }
        }

        private Floor GetClosestFloorWithRequest(Elevator elevator)
        {
            int i = _Random.Next(0, _Floors.Value.Count - 1);
            return _Floors.Value[i];
        }
    }

    public struct ClientsRequestEvent : IRequestEvent
    {
        public Seconds WhenPlanned { get; }
        public Floor Floor { get; }
        public Floor Destination { get; }

        // more properties potentially goes here

        public ClientsRequestEvent(Floor floor, Seconds whenPlanned, Floor destination)
        {
            Floor = floor;
            WhenPlanned = whenPlanned;
            Destination = destination;
        }

        public override string ToString() =>
            $"WhenPlanned: {WhenPlanned}\n" +
            $"Floor: {Floor.Location}\n" +
            $"Destination: {Destination}";
    }

    public class ClientsRequestGenerator
    {
        private readonly Random _Random;
        public ClientsRequestGenerator(Random random)
        {
            _Random = random;
        }

        public List<IRequestEvent> Generate(int count, Floors floors, Seconds maxPlannedTime)
        {
            List<IRequestEvent> requests = new();

            for (int i = 0; i < count; i++)
            {
                requests.Add(
                    new ClientsRequestEvent(
                        GetRandomFloor(floors),
                        new Seconds(_Random.Next(0, maxPlannedTime.Value)),
                        GetRandomFloor(floors)
                    )
                );
            }

            return requests;
        }

        private Floor GetRandomFloor(Floors floors)
        {
            int randomIndex = _Random.Next(0, floors.Value.Count - 1);
            return floors.Value[randomIndex];
        }
    }
}
