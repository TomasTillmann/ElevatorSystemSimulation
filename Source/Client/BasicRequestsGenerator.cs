using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;

namespace Client
{
    public class BasicRequestsGenerator
    {
        private readonly Random _Random;
        public BasicRequestsGenerator(Random random)
        {
            _Random = random;
        }

        public List<IRequestEvent> Generate(int count, Floors floors, Seconds maxPlannedTime)
        {
            List<IRequestEvent> requests = new();

            for(int i = 0; i < count; i++)
            {
                requests.Add(
                    new BasicRequestEvent(
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