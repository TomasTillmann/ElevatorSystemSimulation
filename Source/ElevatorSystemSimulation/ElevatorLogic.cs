using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public abstract class ElevatorLogic<ClientsRequest> : IElevatorLogic where ClientsRequest : IRequestEvent
    {
        public abstract Building Building { get; set; }

        public  void Step(IEvent e)
        {
            if(e is ClientsRequest ce)
            {
                Step(ce);
            }
            else if(e is ElevatorEvent ee)
            {
                Step(ee);
            }
            else
            {
                throw new Exception("Event is something different.");
            }
        }

        protected abstract void Step(ClientsRequest ce);
        protected abstract void Step(ElevatorEvent ee);
    }
}
