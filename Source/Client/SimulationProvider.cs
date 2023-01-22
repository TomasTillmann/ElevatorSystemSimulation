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
                    // just below 1m/s = 3.6km/h - which is considered an average speed of an elevator
                    new Elevator(80.ToCmPerSec(), 5.ToCmPerSec(), 5.ToSeconds(), 10),
                    new Elevator(80.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10),
                    new Elevator(80.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10),
                }
            );

            Building building = new(floors, elevatorSystem);


            SCAN elevatorLogic = new(building);
            //Hungry elevatorLogic = new(building);
            //DestinationDispatch elevatorLogic = new(building);

            // Will be able to pick in the future - TODO
            BasicRequestsGenerator generator = new(new Random(420));
            //

            // run the simulation for 8 hours
            return new Simulation<BasicRequest>(building, elevatorLogic, generator.Generate(250, floors, 28_800.ToSeconds()));
        }
    }
}
