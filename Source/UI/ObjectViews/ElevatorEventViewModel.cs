using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.Generic;

namespace UI
{
    public class ElevatorEventViewModel : ViewModelBase<ElevatorEvent>, IEventViewModel
    {
        public ElevatorViewModel Elevator { get; private set; }
        public FloorViewModel EventLocation { get; private set; }
        public ElevatorAction FinishedAction { get; private set; }
        public int WhenPlanned { get; private set; } 

        public ElevatorEventViewModel(ElevatorEvent model) : base(model)
        {
            Elevator = new ElevatorViewModel(model.Elevator);
            EventLocation = new FloorViewModel(model.EventLocation);
            FinishedAction = model.FinishedAction;
            WhenPlanned = model.WhenPlanned.Value;
        }

        object ICloneable.Clone() { return Clone(); }
        public ElevatorEventViewModel Clone()
        {
            return new ElevatorEventViewModel(Model)
            {
                Elevator = Elevator.Clone(),
                EventLocation = EventLocation.Clone(),
                FinishedAction = FinishedAction,
                WhenPlanned = WhenPlanned
            };
        }
    }
}
