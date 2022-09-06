using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    namespace View
    {
        public class View
        {
            public TextWriter Writer { get; set; }
            public Simulation Simulation { get; }

            public View(Simulation simulation, TextWriter writer)
            {
                Simulation = simulation;
                Writer = writer;
            }

            public void State(IEvent e)
            {
                foreach(Elevator elevator in Simulation.Building.ElevatorSystem.Elevators)
                {
                    Writer.WriteLine(elevator);
                }

                Writer.WriteLine();

                foreach(Floor floor in Simulation.Building.Floors.Value)
                {
                    Writer.WriteLine(floor);
                }
                Writer.WriteLine();

                Writer.WriteLine(e);
                Writer.WriteLine();
                Writer.WriteLine("Current Time: " + Simulation.CurrentTime);

                Writer.WriteLine();
                Writer.WriteLine();
            }
        }
    }
}
