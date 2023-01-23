namespace ElevatorSystemSimulation
{
    public class Population
    {
        public Seconds RequestsTimeSpan { get; set; }
        public int RequestsCount { get; set; }
        public int Seed { get; set; }

        public Population(Seconds requestsTimeSpan, int requestsCount, int seed)
        {
            RequestsTimeSpan = requestsTimeSpan;
            RequestsCount = requestsCount;
            Seed = seed;
        }
    }
}