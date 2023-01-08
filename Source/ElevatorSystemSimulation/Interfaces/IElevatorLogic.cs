namespace ElevatorSystemSimulation
{
    namespace Interfaces
    {
        public interface IElevatorLogic
        {
            Building Building { get; }
            void Execute(ISimulationState state);
        }
        public interface IElevatorLogic<TRequestEvent> : IElevatorLogic where TRequestEvent : Request
        {
            void Execute(ISimulationState<TRequestEvent> requestEvent);
            void Execute(ISimulationState<ElevatorEvent> elevatorEvent);
        }
    }
}
