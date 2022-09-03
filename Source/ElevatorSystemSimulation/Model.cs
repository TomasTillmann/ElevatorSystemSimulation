using DataTypes;
using Extensions;
using Interfaces;

namespace Model
{
    public class Elevator : IElevator /*(client)*/, IPlannableActionableElevator /*(simulation)*/
    {
        public Action<Elevator, Seconds, Centimeters>? PlanElevator { get; set; }
        public Action<Elevator>? UnplanElevator { get; set; }
        public bool IsPlanned { get; set; }
        public CentimetersPerSecond TravelSpeed { get; }
        public CentimetersPerSecond AccelerationDelaySpeed { get; }
        public Seconds DepartingTime { get; }
        public int Capacity { get; }
        public Direction Direction { get; private set; }
        public Centimeters Location { get; set; }
        public Centimeters? maxFloorLocation { get; set; } 
        public Centimeters? minFloorLocation { get; set; } 

        private Elevator(
            CentimetersPerSecond travelSpeed,
            CentimetersPerSecond acceleratingTravelSpeed,
            Seconds departingTime,
            int capacity,
            IFloor? startingFloor,
            IFloor? maxFloorLocation = null,
            IFloor? minFloorLocation = null)
        {
            TravelSpeed = travelSpeed;
            AccelerationDelaySpeed = acceleratingTravelSpeed;
            DepartingTime = departingTime;
            Capacity = capacity;
            Location = startingFloor != null ? startingFloor.Location : 0.ToCentimeters();
        }

        public static IElevator Get(
            CentimetersPerSecond travelSpeed,
            CentimetersPerSecond acceleratingTravelSpeed,
            Seconds departingTime,
            int capacity,
            IFloor? startingFloor,
            IFloor? maxFloorLocation = null,
            IFloor? minFloorLocation = null)
        {
            return new Elevator(
            travelSpeed,
            acceleratingTravelSpeed,
            departingTime,
            capacity,
            startingFloor,
            maxFloorLocation = null,
            minFloorLocation = null);
        }

        //TODO - floor in building dont like it - should be hiddne from client
        public void MoveTo(IFloor? floor)
        {
            if(floor == null)
            {
                return;
            }

            SetDirection(floor.Location);
            PlanMe(GetDistance(floor.Location) / TravelSpeed, floor.Location);
        }

        public void Idle()
        {
            PlanMe(0.ToSeconds(), Location);
        }

        public void Load()
        {
            //TODO - IMPLEMENT: depart out + depart in time - maybe no one to depart out or no one to depart in on the floor 
            PlanMe(DepartingTime, Location);
        }

        private void PlanMe(Seconds duration, Centimeters location)
        {
            if(PlanElevator == null)
            {
                throw new Exception("PlanElevator function is not set.");
            }

            PlanElevator(this, duration, location);
        }

        private void UnplanMe(Seconds duration, Centimeters location)
        {
            if(UnplanElevator == null)
            {
                throw new Exception("UnplanElevator function is not set.");
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

    public class Floor : IFloor
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

    public class Building : IBuilding
    {
        public IFloors Floors { get; set; }
        public IElevatorSystem ElevatorSystem { get; set; }
        public IPopulation? Population { get; set; }

        public Building(Floors floors, ElevatorSystem elevatorSystem, Population? population = null)
        {
            Floors = floors;
            ElevatorSystem = elevatorSystem;
            Population = population;
        }
    }

    public class Floors : IFloors
    {
        public List<IFloor> Value { get; } = new();
        public Centimeters InBetweenFloorsSpace { get; }

        public Floors(List<IFloor> value, Centimeters inBetweenFloorsSpace)
        {
            InBetweenFloorsSpace = inBetweenFloorsSpace;

            // avoiding sorting value too
            IFloor[] tempValue = new IFloor[value.Count]; 
            value.CopyTo(tempValue);
            Value = tempValue.ToList();
            //

            Value.Sort((f1, f2) => f1.FloorId.CompareTo(f2.FloorId));
            SetFloorsLocation();
        }

        public IFloor? GetFloorById(int floorId)
        {
            return Value.Find(floor => floor.FloorId == floorId);
        }

        private void SetValue()
        {

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

    public class ElevatorSystem : IElevatorSystem
    {
        public List<IElevator> Elevators { get; set; } = new();

        public ElevatorSystem(List<IElevator> elevators)
        {
            Elevators = elevators;
        }
    }

    public class ElevatorSystemInBuilding : ElevatorSystem
    {
        public Centimeters LowestLocation { get; }
        public Centimeters HighestLocation { get; }

        public ElevatorSystemInBuilding(List<IElevator> elevators, Centimeters lowestLocation, Centimeters highestLocation)
        :base(elevators)
        {
            LowestLocation = lowestLocation;
            HighestLocation = highestLocation;
        }
    }

    public class Population : IPopulation
    {
        public List<PopulationDistribution> Distribution { get; set; }
        public int TotalPeopleCount { get; set; }
        public int AveragePeopleCount { get; set; }


        public Population(List<PopulationDistribution> distribution)
        {
            Distribution = distribution;
        }
    }

    public class Statistics
    {
        public int AverageWaitingTime { get; set; }
        public int LongestWaitingTime { get; set; }
        public int PeopleDepartedCount { get; set; }
        public int AverageElevatorTravelInFloors { get; set; }
        private List<int> _AllWaitingTimes = new();
        public List<int> AllWaitingTimes
        {
            get
            {
                _AllWaitingTimes.Sort();
                return _AllWaitingTimes;
            }
        }
    }
}