using ElevatorSystemSimulation;
using Model;
using DataTypes;
using Interfaces;
using Extensions;

namespace Client
{
    public class ClientsElevatorIncredbleAlgorithm : IElevatorLogic
    {
        public IBuilding Building { get; set; } 
        private List<IElevator> _Elevators { get; set; }
        private Floors _Floors { get; set; }
        private Random _Random { get; }

        public ClientsElevatorIncredbleAlgorithm(IBuilding building)
        {
            Building = building;
            _Elevators = building.ElevatorSystem.Elevators;
            _Floors = (Floors)building.Floors;
            _Random = new Random();
        }

        public void Step(IEvent e)
        {
            // randomly moves the first elevator
            IElevator elevator = _Elevators[0];
            elevator.MoveTo(_Floors.GetFloorById(_Random.Next(0,9)));
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Floors floors = new(
                new List<IFloor>()
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
                new List<IElevator>()
                {
                    Elevator.Get(20.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    Elevator.Get(20.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                }
            );

            Building building = new(floors, elevatorSystem);
            ClientsElevatorIncredbleAlgorithm elevatorLogic = new(building);

            Simulation simulation = new(building, elevatorLogic, 100_000_000.ToSeconds(), new List<IRequest>()
            {
                new Request(),
                new Request(),
                new Request(),
                new Request(),
                new Request(),
                new Request(),
                new Request(),
            });
            simulation.Run();
        }
    }
}