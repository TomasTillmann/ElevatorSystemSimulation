using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public abstract class ActionAfterElevatorEvent<TContextType> : StateDecisionTreeActionNode<ISimulationState<ElevatorEvent>, TContextType> where TContextType : IElevatorLogic
    {
        protected ActionAfterElevatorEvent(TContextType context) : base(context) { }
    }
}
