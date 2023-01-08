using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public abstract class ConditionAfterElevatorEvent<TContextType> : StateDecisionTreeConditionNode<ISimulationState<ElevatorEvent>, TContextType> where TContextType : IElevatorLogic
    {
        protected ConditionAfterElevatorEvent(
            IStateDecisionTreeNode<ISimulationState<ElevatorEvent>, TContextType> onTrue,
            IStateDecisionTreeNode<ISimulationState<ElevatorEvent>, TContextType> onFalse,
            TContextType context
        )
        : base(onTrue, onFalse, context) { }

        protected ConditionAfterElevatorEvent(TContextType context) : base(context) { }
    }
}
