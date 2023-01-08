using ElevatorSystemSimulation.Interfaces;
using System;

namespace UI
{
    public interface IEventViewModel : ICloneable
    {
        FloorViewModel EventLocation { get; }
    }
}
