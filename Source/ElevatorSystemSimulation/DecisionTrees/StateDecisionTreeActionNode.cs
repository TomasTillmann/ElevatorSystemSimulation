namespace ElevatorSystemSimulation
{
    public abstract class StateDecisionTreeActionNode<StateType, ContextType> : IStateDecisionTreeNode<StateType, ContextType>
    {
        public ContextType Context { get; }

        protected StateDecisionTreeActionNode(ContextType context)
        {
            Context = context;
        }

        /// Should return true if executed successfully
        public abstract bool Execute(StateType state);
    }
}
