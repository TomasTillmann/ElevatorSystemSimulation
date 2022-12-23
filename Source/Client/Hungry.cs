using ElevatorSystemSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Hungry : ElevatorLogic<BasicRequestEvent>
    {
        public Hungry(Building building) : base(building) { }

        protected override void Execute(SimulationState<BasicRequestEvent> state)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(SimulationState<ElevatorEvent> state)
        {
            throw new NotImplementedException();
        }
    }
}
