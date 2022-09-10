using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public abstract class ElevatorLogic<RequestEvent> : IElevatorLogic where RequestEvent : IRequestEvent
    {
        public Building Building { get; }

        public ElevatorLogic(Building building)
        {
            Building = building;
        }

        public void Step(IEvent e)
        {
            if(e is RequestEvent ce)
            {
                ce.Floor._Requests.Add(ce);
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

        public IEnumerable<RequestEvent> GetAllCurrentRequestEvents()
        {
            foreach(Floor floor in Building.Floors.Value)
            {
                foreach(RequestEvent requestEvent in floor.Requests)
                {
                    yield return requestEvent;
                }
            }
        }

        protected abstract void Step(RequestEvent ce);
        protected abstract void Step(ElevatorEvent ee);
    }
}
