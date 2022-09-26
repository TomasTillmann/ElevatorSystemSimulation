using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystemSimulation
{
    public interface IStateDecisionTreeNode<StateType, ContextType>
    {
        public ContextType Context { get; }
        public bool Execute(StateType state);
    }

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

    public abstract class StateDecisionTreeConditionNode<StateType, ContextType> : IStateDecisionTreeNode<StateType, ContextType>
    {
        public ContextType Context { get; }

        public IStateDecisionTreeNode<StateType, ContextType>? OnTrue { get; set; }
        public IStateDecisionTreeNode<StateType, ContextType>? OnFalse { get; set; }

        protected StateDecisionTreeConditionNode(ContextType context)
        {
            Context = context;
        }

        protected StateDecisionTreeConditionNode(IStateDecisionTreeNode<StateType, ContextType> onTrue, IStateDecisionTreeNode<StateType, ContextType> onFalse, ContextType context)
        {
            Context = context;
            OnTrue = onTrue;
            OnFalse = onFalse;
        }

        protected abstract bool Predicate(StateType state);

        /// Decide what node to execute based on predicate.
        /// Should return true if and only if action node executes successfully 
        public bool Execute(StateType state)
        {
            if (Predicate(state))
            {
                if (OnTrue is null)
                {
                    return false;
                }

                return OnTrue.Execute(state);
            }
            else
            {
                if (OnFalse is null)
                {
                    return false;
                }

                return OnFalse.Execute(state);
            }
        }
    }
}
