using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public static class ElevatorFactory
    {
        public static IElevatorView GetIElevatorView(
            CentimetersPerSecond travelSpeed,
            CentimetersPerSecond acceleratingTravelSpeed,
            Seconds departingTime,
            int capacity,
            Floor? startingFloor)
        {
            return new Elevator(
            travelSpeed,
            acceleratingTravelSpeed,
            departingTime,
            capacity,
            startingFloor);
        }
    }

    public class Elevator : IElevatorView /*(client)*/, IPlannableActionableElevator /*(simulation)*/
    {
        public Action<Elevator, Seconds, Floor>? PlanElevator { get; set; }
        public Action<Elevator>? UnplanElevator { get; set; }
        public bool IsAvailable { get; set; } = true;
        public CentimetersPerSecond TravelSpeed { get; }
        public CentimetersPerSecond AccelerationDelaySpeed { get; }
        public Seconds DepartingTime { get; }
        public int Capacity { get; }
        public Direction Direction { get; private set; }
        public Centimeters Location { get; set; }

        // client cant make instances directly - internal
        internal Elevator(
            CentimetersPerSecond travelSpeed,
            CentimetersPerSecond acceleratingTravelSpeed,
            Seconds departingTime,
            int capacity,
            Floor? startingFloor)
        {
            TravelSpeed = travelSpeed;
            AccelerationDelaySpeed = acceleratingTravelSpeed;
            DepartingTime = departingTime;
            Capacity = capacity;
            Location = startingFloor != null ? startingFloor.Location : 0.ToCentimeters();
        }

        //TODO - floor in building dont like it - should be hiddne from client
        public void MoveTo(Floor? floor)
        {
            if (floor == null)
            {
                return;
            }

            SetDirection(floor.Location);
            PlanMe(GetDistance(floor.Location) / TravelSpeed, floor);
        }

        public void Idle(Floor? floor)
        {
            if(floor == null)
            {
                return;
            }

            if (Location != floor.Location)
            {
                throw new Exception("Elevator cannot Idle. Elevators can Idle only when fully in a floor");
            }

            PlanMe(0.ToSeconds(), floor);
        }

        public void Load(Floor? floor)
        {
            if(floor == null)
            {
                return;
            }

            if (Location != floor.Location)
            {
                throw new Exception("Elevator cannot load people. Elevators can load people only when fully in a floor");
            }

            //TODO - IMPLEMENT: depart out + depart in time - maybe no one to depart out or no one to depart in on the floor 
            PlanMe(DepartingTime, floor);
        }

        private void PlanMe(Seconds duration, Floor destination)
        {
            if (PlanElevator == null)
            {
                throw new Exception("Elevator cannot make this action. It is not in simulation yet.");
            }

            PlanElevator(this, duration, destination);
        }

        private void UnplanMe(Seconds duration, Centimeters location)
        {
            if (UnplanElevator == null)
            {
                throw new Exception("Elevator cannot make this action. It is not in simulation yet.");
            }

            UnplanElevator(this);
        }

        private Centimeters GetDistance(Centimeters floorLocation)
        {
            return new Centimeters(Math.Abs((floorLocation - Location).Value));
        }

        private void SetDirection(Centimeters floorLocation)
        {
            Direction = (floorLocation - Location).Value > 0 ?
                Direction.Up :
                Direction.Down;
        }
    }

    public class Floor
    {
        public Centimeters Location { get; set; }
        public int FloorId { get; }
        public Centimeters Height { get; }
        public string? Name { get; }

        public Floor(int floorId, Centimeters height, string? name = null)
        {
            FloorId = floorId;
            Height = height;
            Name = name;
        }
    }

    public class Building
    {
        public Floors Floors { get; set; }
        public ElevatorSystem ElevatorSystem { get; set; }

        public Building(Floors floors, ElevatorSystem elevatorSystem)
        {
            Floors = floors;
            ElevatorSystem = elevatorSystem;
        }
    }

    public class Floors
    {
        public List<Floor> Value { get; } = new();
        public Centimeters InBetweenFloorsSpace { get; }

        public Floors(List<Floor> value, Centimeters inBetweenFloorsSpace)
        {
            InBetweenFloorsSpace = inBetweenFloorsSpace;

            // avoiding sorting value too
            foreach(Floor floor in value)
            {
                Value.Add(floor);
            }
            //

            Value.Sort((f1, f2) => f1.FloorId.CompareTo(f2.FloorId));
            SetFloorsLocation();
        }

        public Floor? GetFloorById(int floorId)
        {
            return Value.Find(floor => floor.FloorId == floorId);
        }

        private void SetFloorsLocation()
        {
            Centimeters totalHeight = 0.ToCentimeters();

            foreach (Floor floor in Value)
            {
                floor.Location = totalHeight;
                totalHeight += floor.Height;
                totalHeight += InBetweenFloorsSpace;
            }
        }
    }

    public class ElevatorSystem
    {
        public List<IElevatorView> Elevators { get; set; } = new();

        public ElevatorSystem(List<IElevatorView> elevators)
        {
            Elevators = elevators;
        }
    }
}