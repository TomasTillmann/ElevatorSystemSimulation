using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using ElevatorSystemSimulation.Extensions;

namespace Client
{
    // Main logic - implements client - there will be available catalog of some logics
    public class ClientsElevatorIncredbleAlgorithm : IElevatorLogic<ClientsAmazingRequestEvent>
    {
        private List<IElevatorView> _Elevators { get; set; }
        private Floors _Floors { get; set; }
        private Random _Random { get; }

        public Building Building { get; set; } 

        public ClientsElevatorIncredbleAlgorithm(Building building)
        {
            Building = building;
            _Elevators = building.ElevatorSystem.Elevators;
            _Floors = building.Floors;
            _Random = new Random();
        }

        // TODO: fix: ugly but necessary
        public void Step(IEvent e)
        {
            switch (e)
            {
                case  IRequestEvent re:
                    Step(re);
                    break;

                case  IElevatorEvent ee:
                    Step(ee);
                    break;
            }
        }

        public void Step(ClientsAmazingRequestEvent e)
        {
            IElevatorView? freeElevator = _Elevators.Find(elevator => elevator.IsAvailable);
            if(freeElevator != null)
            {
                freeElevator.MoveTo(e.Floor);
            }

            //TODO: some list of unresolved requests or something
        }

        public void Step(IElevatorEvent e)
        {
            IElevatorView elevator = e.Elevator;
            elevator.MoveTo(_Floors.GetFloorById(_Random.Next(0,9)));
        }
    }

    // implements client - represents capabilities of the elevator system - used only in the clients logic
    // - IElevatorLogic - hence independent of the whole simulation -> very customizable elevator systems - no constraints basically
    public struct ClientsAmazingRequestEvent : IRequestEvent
    {
        public Floor Floor { get; } 
        public Seconds WhenPlanned { get; } 
        public Floor FloorDestination { get; }

        public ClientsAmazingRequestEvent(Floor floor, Seconds whenPlanned, Floor floorDestination)
        {
            Floor = floor;
            WhenPlanned = whenPlanned;
            FloorDestination = floorDestination;
        }
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
                },
                10.ToCentimeters()
            );

            ElevatorSystem elevatorSystem = new ElevatorSystem(
                new List<IElevatorView>()
                {
                    ElevatorFactory.GetIElevatorView(20.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    ElevatorFactory.GetIElevatorView(20.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                }
            );

            var e = ElevatorFactory.GetIElevatorView(20.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0));
            e.MoveTo(floors.GetFloorById(1));

            Building building = new(floors, elevatorSystem);
            ClientsElevatorIncredbleAlgorithm elevatorLogic = new(building);
            ClientsAmazingGenerator generator = new(new Random());
            Seconds totalSimulationRunningTime = 10_000.ToSeconds();

            Simulation simulation = new(building, elevatorLogic, totalSimulationRunningTime, generator.Generate(30, floors, totalSimulationRunningTime));
            simulation.Run();

            Statistics result = simulation.Statistics;
            //Console.WriteLine(result);
        }
    }
}