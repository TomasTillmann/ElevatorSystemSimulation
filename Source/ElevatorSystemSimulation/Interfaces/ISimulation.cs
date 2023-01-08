namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface ISimulation
        {
            Building Building { get; }
            Seconds CurrentTime { get; }
            IEvent? LastEvent { get; }
            IEvent? LastAction { get; }
            bool IsOver { get; }
            int StepCount { get; }

            void Run();
            void Step();
            StatisticsResult GetStatistics();
        }

        public interface ISimulation<TRequest> : ISimulation, IRestartable where TRequest : Request
        {
            IReadOnlyList<TRequest> AllRequests { get; }
            IReadOnlyList<TRequest> DepartedRequests { get; }

        }
    }
}
