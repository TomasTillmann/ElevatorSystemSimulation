using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using System.Collections.Generic;

namespace UI
{
    public class FloorViewModel : ViewModelBase<Floor>
    {
        public Centimeters Height { get; set; }
        public IReadOnlyCollection<IRequestEvent> Requests { get; set; } 
        public int Id { get; }

        public FloorViewModel(Floor floor)

        :base(floor)
        {
            Height = floor.Height;
            Requests = floor.Requests;
            Id = floor.FloorId;
        }
    }
}
