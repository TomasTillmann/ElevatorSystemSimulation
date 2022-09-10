﻿using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.View;

namespace Client
{
    // Main logic - implements client - there will be available catalog of some logics
    public class ClientsElevatorIncredbleAlgorithm : ElevatorLogic<ClientsAmazingRequestEvent> 
    {
        private List<Elevator> _Elevators { get; set; }
        private Floors _Floors { get; set; }
        private Random _Random { get; }
        private readonly Dictionary<ElevatorAction, Action<ElevatorEvent>> _DoAfterElevatorAction = new();


        //TODO: dirty - will be deleted
        public View? View { get; set; }

        public ClientsElevatorIncredbleAlgorithm(Building building)
        :base(building)
        {
            _Elevators = building.ElevatorSystem.Elevators;
            _Floors = building.Floors;
            _Random = new Random();

            _DoAfterElevatorAction[ElevatorAction.MoveTo] += StepAfterMove;
            _DoAfterElevatorAction[ElevatorAction.UnloadAndLoad] += StepAfterUnloadAndLoad;
            _DoAfterElevatorAction[ElevatorAction.Idle] += StepAfterIdle;
        }

        protected override void Step(ClientsAmazingRequestEvent e)
        {
            Elevator? freeElevator = _Elevators.Find(elevator => elevator.IsIdle);

            if(freeElevator != null)
            {
                freeElevator.MoveTo(e.Floor);
            }
        }

        protected override void Step(ElevatorEvent e)
        {
            _DoAfterElevatorAction[e.FinishedAction].Invoke(e);
        }

        //TODO - refactor and implement and run and test ...
        private void StepAfterMove(ElevatorEvent e)
        {
            if(e.Elevator.AttendingRequests.Count > 0 || e.CurrentFloor.Requests.Count > 0)
            {
                e.Elevator.UnloadAndLoad(e.CurrentFloor);
            }
            else if(GetAllCurrentRequestEvents().Any())
            {
                e.Elevator.MoveTo(GetClosestFloorWithRequest(e.Elevator));
            }
            else
            {
                e.Elevator.Idle(e.CurrentFloor);

                Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

                if(freeElevator != null)
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

                if(freeElevator != null)
                {
                    freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
                }
            }
        }

        private void StepAfterIdle(ElevatorEvent e)
        {
            Elevator? freeElevator = _Elevators.Find(e => e.IsIdle);

            if(freeElevator != null)
            {
                freeElevator.MoveTo(GetClosestFloorWithRequest(freeElevator));
            }
        }

        private Floor GetClosestFloorWithRequest(Elevator elevator)
        {
            int i = _Random.Next(0, _Floors.Value.Count - 1);
            return _Floors.Value[i];
        }
        //
    }

    // implements client - represents capabilities of the elevator system - used only in the clients logic
    // - IElevatorLogic - hence independent of the whole simulation -> very customizable elevator systems - no constraints basically
    public struct ClientsAmazingRequestEvent : IRequestEvent
    {
        public Seconds WhenPlanned { get; } 
        public Floor Floor { get; } 
        public Floor Destination { get; }

        public ClientsAmazingRequestEvent(Floor floor, Seconds whenPlanned, Floor destination)
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

    // Optional, but very useful
    public class ClientsAmazingGenerator
    {
        private readonly Random _Random;
        public ClientsAmazingGenerator(Random random)
        {
            _Random = random;
        }

        public List<IRequestEvent> Generate(int count, Floors floors, Seconds maxPlannedTime)
        {
            List<IRequestEvent> requests = new();

            for(int i = 0; i < count; i++)
            {
                requests.Add(
                    new ClientsAmazingRequestEvent(
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

    public class Program
    {
        public static void Main(string[] args)
        {
            Floors floors = new(
                new List<Floor>()
                {
                    new Floor(0, 250.ToCentimeters()),
                    new Floor(1, 250.ToCentimeters()),
                    new Floor(2, 250.ToCentimeters()),
                    new Floor(3, 250.ToCentimeters()),
                    new Floor(4, 250.ToCentimeters()),
                    new Floor(5, 250.ToCentimeters()),
                    new Floor(6, 250.ToCentimeters()),
                    new Floor(7, 250.ToCentimeters()),
                    new Floor(8, 250.ToCentimeters()),
                    new Floor(9, 250.ToCentimeters()),
                    new Floor(10, 250.ToCentimeters()),
                    new Floor(11, 250.ToCentimeters()),
                    new Floor(12, 250.ToCentimeters()),
                    new Floor(13, 250.ToCentimeters()),
                    new Floor(14, 250.ToCentimeters()),
                    new Floor(15, 250.ToCentimeters()),
                    new Floor(16, 250.ToCentimeters()),
                    new Floor(17, 250.ToCentimeters()),
                    new Floor(18, 250.ToCentimeters()),
                    new Floor(19, 250.ToCentimeters()),
                    new Floor(20, 250.ToCentimeters()),
                    new Floor(21, 250.ToCentimeters()),
                    new Floor(22, 250.ToCentimeters()),
                    new Floor(23, 250.ToCentimeters()),
                    new Floor(24, 250.ToCentimeters()),
                    new Floor(25, 250.ToCentimeters()),
                    new Floor(26, 250.ToCentimeters()),
                    new Floor(27, 250.ToCentimeters()),
                    new Floor(28, 250.ToCentimeters()),
                },
                10.ToCentimeters()
            );

            ElevatorSystem elevatorSystem = new ElevatorSystem(
                new List<Elevator>()
                {
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                }
            );

            Building building = new(floors, elevatorSystem);
            ClientsElevatorIncredbleAlgorithm elevatorLogic = new(building);
            ClientsAmazingGenerator generator = new(new Random());
            Seconds totalSimulationRunningTime = 1000.ToSeconds();

            Simulation simulation = new(building, elevatorLogic, totalSimulationRunningTime, generator.Generate(60, floors, totalSimulationRunningTime));

            //TODO: this visualizing is dirty - logic shouldnt care about visualizing
            elevatorLogic.View = new View(simulation, Console.Out);

            simulation.Run();
        }
    }
}