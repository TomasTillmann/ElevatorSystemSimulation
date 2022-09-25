using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.Generic;

namespace UI
{
    public class FloorViewModel : ViewModelBase<Floor>, ICloneable
    {
        public int Id { get; }
        public Centimeters Height { get; }

        public List<IRequestEvent> Requests { get; set; } 

        public FloorViewModel(Floor floor)
        :base(floor)
        {
            Height = floor.Height;
            Requests = (List<IRequestEvent>)floor.Requests;
            Id = floor.Id;
        }

        object ICloneable.Clone() { return Clone(); }
        public FloorViewModel Clone()
        {
            return new FloorViewModel(Model)
            {
                Requests = new List<IRequestEvent>(Requests)
            };
        }
    }
}
