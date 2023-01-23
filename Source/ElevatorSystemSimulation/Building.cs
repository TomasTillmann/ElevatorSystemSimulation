namespace ElevatorSystemSimulation
{
    public class Building
    {
        public Floors Floors { get; set; }
        public ElevatorSystem ElevatorSystem { get; set; }
        public Population Population { get; set; }

        public Building(Floors floors, ElevatorSystem elevatorSystem, Population population)
        {
            Floors = floors;
            ElevatorSystem = elevatorSystem;
            Population = population;
        }
    }
}