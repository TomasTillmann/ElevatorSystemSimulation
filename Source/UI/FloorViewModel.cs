using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using System.Collections.Generic;

namespace UI
{
    public class FloorViewModel : ViewModelBase<Floor>
    {
        public Centimeters Height { get; set; }
        public List<IRequestEvent> Requests { get; set; } 
        public int Id { get; }

        public FloorViewModel(Floor floor)

        :base(floor)
        {
            Height = floor.Height;
            Requests = (List<IRequestEvent>)floor.Requests;
            Id = floor.Id;
        }
    }
}
