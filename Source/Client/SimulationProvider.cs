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
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                    new Floor(250.ToCentimeters()),
                },
                10.ToCentimeters()
            );

            ElevatorSystem elevatorSystem = new ElevatorSystem(
                new List<Elevator>()
                {
                    new Elevator(50.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10),
                    new Elevator(50.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10),
                }
            );

            Building building = new(floors, elevatorSystem);
            SCAN elevatorLogic = new(building);
            BasicRequestsGenerator generator = new(new Random(420));
            Seconds totalSimulationRunningTime = 1000.ToSeconds();

            return new Simulation(building, elevatorLogic, totalSimulationRunningTime, generator.Generate(150, floors, totalSimulationRunningTime));
        }
    }
}
