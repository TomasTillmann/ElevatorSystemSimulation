using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public class Floors
    {
        public List<Floor> Value { get; } = new();
        public Floor HeighestFloor => Value[Value.Count - 1];
        public Floor LowestFloor => Value[0]; 

        //public Centimeters InBetweenFloorsSpace { get; }

        public Floors(List<Floor> value, Centimeters inBetweenFloorsSpace)
        {
            //InBetweenFloorsSpace = inBetweenFloorsSpace;

            Value = new List<Floor>(value);
            Value.Sort((f1, f2) => f1.Id.CompareTo(f2.Id));

            int i = -1;
            Value.ForEach(f => f.Id = i+=1);

            SetFloorsLocation();
        }

        public Floor? GetFloorById(int floorId)
        {
            return Value.Find(floor => floor.Id == floorId);
        }

        public IEnumerable<Interfaces.Request> GetAllActiveRequests()
        {
            foreach(Floor floor in Value)
            {
                foreach(Interfaces.Request request in floor.Requests)
                {
                    yield return request;
                }
            }
        }

        private void SetFloorsLocation()
        {
            Centimeters totalHeight = 0.ToCentimeters();

            foreach (Floor floor in Value)
            {
                floor.Location = totalHeight;
                totalHeight += floor.Height;
                //totalHeight += InBetweenFloorsSpace;
            }
        }
    }
}