namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface ISimulation : IRestartable
        {
            Building Building { get; }
            Seconds CurrentTime { get; }
            IEvent? LastEvent { get; }
            IEvent? LastAction { get; }
            bool IsOver { get; }
            int StepCount { get; }

            void Run();
            void Step();
        }
    }
}
