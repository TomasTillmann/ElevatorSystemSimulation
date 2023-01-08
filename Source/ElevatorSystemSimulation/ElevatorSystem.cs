using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public class ElevatorSystem
    {
        public List<Elevator> Elevators { get; set; } = new();

        public ElevatorSystem(List<Elevator> value)
        {
            Elevators = value;

            int i = -1;
            value.ForEach(e => e.Id = i += 1);
        }

        public IEnumerable<Request> GetAllInElevatorRequests()
        {
            foreach(Elevator elevator in Elevators)
            {
                foreach(Request request in elevator.AttendingRequests)
                {
                    yield return request;
                }
            }
        }

        internal void ValidateElevatorsLocations(Floors floors)
        {
            foreach (Elevator elevator in Elevators)
            {
                if (elevator.Location > floors.HeighestFloor.Location ||
                    elevator.Location < floors.LowestFloor.Location)
                {
                    throw new Exception("Elevators are out of the bounds of the building!");
                }
            }
        }
    }
}