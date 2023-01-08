using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public abstract class ConditionAfterRequestEvent<TContextType, TRequestType> : StateDecisionTreeConditionNode<ISimulationState<TRequestType>, TContextType> where TContextType : IElevatorLogic where TRequestType : Interfaces.Request 
    {
        protected ConditionAfterRequestEvent(
            IStateDecisionTreeNode<ISimulationState<TRequestType>, TContextType> onTrue,
            IStateDecisionTreeNode<ISimulationState<TRequestType>, TContextType> onFalse,
            TContextType context
        )
        : base(onTrue, onFalse, context) { }

        protected ConditionAfterRequestEvent(TContextType context) : base(context) { }
    }
}
