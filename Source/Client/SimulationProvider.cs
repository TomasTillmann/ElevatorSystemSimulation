using System;
using ElevatorSystemSimulation;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSystemSimulation.Extensions;

namespace Client
{
    /// <summary>
    /// This is used on startup.
    /// </summary>
    public class SimulationProvider
    {
        public static Simulation<BasicRequest> GetSimulation()
        {
            //// User picks

            // only the same floors are allowed for now! - TODO
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
                    new Elevator(50.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10),
                }
            );

            Building building = new(floors, elevatorSystem);


            Hungry elevatorLogic = new(building);
            //DestinationDispatch elevatorLogic = new(building);

            // Will be able to pick in the future - TODO
            BasicRequestsGenerator generator = new(new Random(420));
            //

            return new Simulation<BasicRequest>(building, elevatorLogic, generator.Generate(500, floors, 5000.ToSeconds()));
        }
    }
}
