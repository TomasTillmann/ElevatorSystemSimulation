using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    public class Floor : IRestartable, ILocatable
    {
        #region Identification
        public int Id { get; internal set; }
        public string? Name { get; }

        #endregion

        #region Simulation

        public IReadOnlyCollection<Request> Requests => _Requests; 
        internal readonly List<Request> _Requests = new();

        public IReadOnlyCollection<Elevator> PlannedElevators => _PlannedElevators;
        internal readonly List<Elevator> _PlannedElevators = new();

        public void Restart()
        {
            _Requests.Clear();
        }

        #endregion

        public Centimeters Height { get; }
        public Centimeters Location { get; set; }

        public Floor(Centimeters height, string? name = null)
        {
            Height = height;
            Name = name;
        }

        public override string ToString() => 
            $"FloorId: {Id}\n" +
            $"FloorLocation: {Location}";
    }
}