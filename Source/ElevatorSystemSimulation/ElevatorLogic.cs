using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public abstract class ElevatorLogic<RequestEvent> : IElevatorLogic where RequestEvent : IRequestEvent
    {
        private List<RequestEvent> _Requests { get; } = new();

        protected abstract Building Building { get; set; }
        protected IReadOnlyCollection<RequestEvent> Requests => _Requests;

        public void Step(IEvent e)
        {
            if(e is RequestEvent ce)
            {
                _Requests.Add(ce);
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

        protected abstract void Step(RequestEvent ce);
        protected abstract void Step(ElevatorEvent ee);
    }
}
