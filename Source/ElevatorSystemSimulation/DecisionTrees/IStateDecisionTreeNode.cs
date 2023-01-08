namespace ElevatorSystemSimulation
{
    public interface IStateDecisionTreeNode<StateType, ContextType>
    {
        public ContextType Context { get; }
        public bool Execute(StateType state);
    }
}
