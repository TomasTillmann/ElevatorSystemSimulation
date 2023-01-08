using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public abstract class ActionAfterRequestEvent<TContextType, TRequestType> : StateDecisionTreeActionNode<ISimulationState<TRequestType>, TContextType> where TContextType : IElevatorLogic where TRequestType : Interfaces.Request
    {
        protected ActionAfterRequestEvent(TContextType context) : base(context) { }
    }
}
