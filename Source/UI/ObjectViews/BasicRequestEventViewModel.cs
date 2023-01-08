using Client;
using ElevatorSystemSimulation.Interfaces;
using System;

namespace UI
{
    // tight coupling between clients request of choice - cannot be avoided unfortunately, if its desired to show it in UI
    // if new request event in the future, just add new viewmodel and new view / datatemplate for it, which is actaully the best outcome to hope for i think
    public class BasicRequestEventViewModel : ViewModelBase<BasicRequest>, IEventViewModel
    {
        public FloorViewModel EventLocation { get; private set; }
        public FloorViewModel Destination { get; private set; }

        public BasicRequestEventViewModel(BasicRequest model) : base(model)
        {
            EventLocation = new FloorViewModel(model.EventLocation);
            Destination = new FloorViewModel(model.Destination);
        }

        object ICloneable.Clone() { return Clone(); }
        public BasicRequestEventViewModel Clone()
        {
            return new BasicRequestEventViewModel(Model)
            {
                EventLocation = EventLocation.Clone(),
                Destination = Destination.Clone()
            };
        }
    }
}
