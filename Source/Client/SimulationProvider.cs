using System;
using ElevatorSystemSimulation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSystemSimulation.Extensions;

namespace Client
{
    public class SimulationProvider
    {
        public static Simulation GetSimulation()
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
                    new Elevator(50.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    new Elevator(50.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(14)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                    //new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                    //new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                }
            );

            Building building = new(floors, elevatorSystem);
            BasicElevatorLogic elevatorLogic = new(building);
            BasicRequestsGenerator generator = new(new Random(420));
            Seconds totalSimulationRunningTime = 1000.ToSeconds();

            return new Simulation(building, elevatorLogic, totalSimulationRunningTime, generator.Generate(150, floors, totalSimulationRunningTime));
        }
    }
}
