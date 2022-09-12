using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace UI
{
    public class MainViewModel : DependencyObject
    {
        private Simulation _Simulation { get; }

        public List<Elevator> Elevators { get => (List<Elevator>)GetValue(ElevatorsProperty); set => SetValue(ElevatorsProperty, value); }
        public static readonly DependencyProperty ElevatorsProperty = DependencyProperty.Register("Elevators", typeof(List<Elevator>), typeof(MainViewModel));

        public List<Floor> Floors { get => (List<Floor>)GetValue(FloorsProperty); set => SetValue(FloorsProperty, value); }
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register("Floors", typeof(List<Floor>), typeof(MainViewModel));

        public MainViewModel()
        {
            _Simulation = GetSimulation();
            Elevators = _Simulation.Building.ElevatorSystem.Elevators;
            Floors = _Simulation.Building.Floors.Value;
        }

        private Simulation GetSimulation()
        {
            Floors floors = new(
                new List<Floor>()
                {
                    new Floor(0, 250.ToCentimeters()),
                    new Floor(1, 250.ToCentimeters()),
                    new Floor(2, 250.ToCentimeters()),
                    new Floor(3, 250.ToCentimeters()),
                    new Floor(4, 250.ToCentimeters()),
                    new Floor(5, 250.ToCentimeters()),
                    new Floor(6, 250.ToCentimeters()),
                    new Floor(7, 250.ToCentimeters()),
                    new Floor(8, 250.ToCentimeters()),
                    new Floor(9, 250.ToCentimeters()),
                    new Floor(10, 250.ToCentimeters()),
                    new Floor(11, 250.ToCentimeters()),
                    new Floor(12, 250.ToCentimeters()),
                    new Floor(13, 250.ToCentimeters()),
                    new Floor(14, 250.ToCentimeters()),
                    new Floor(15, 250.ToCentimeters()),
                    new Floor(16, 250.ToCentimeters()),
                    new Floor(17, 250.ToCentimeters()),
                    new Floor(18, 250.ToCentimeters()),
                    new Floor(19, 250.ToCentimeters()),
                    new Floor(20, 250.ToCentimeters()),
                    new Floor(21, 250.ToCentimeters()),
                    new Floor(22, 250.ToCentimeters()),
                    new Floor(23, 250.ToCentimeters()),
                    new Floor(24, 250.ToCentimeters()),
                    new Floor(25, 250.ToCentimeters()),
                    new Floor(26, 250.ToCentimeters()),
                    new Floor(27, 250.ToCentimeters()),
                    new Floor(28, 250.ToCentimeters()),
                },
                10.ToCentimeters()
            );

            ElevatorSystem elevatorSystem = new ElevatorSystem(
                new List<Elevator>()
                {
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                }
            );

            Building building = new(floors, elevatorSystem);
            ClientsElevatorLogic elevatorLogic = new(building);
            ClientsRequestGenerator generator = new(new Random());
            Seconds totalSimulationRunningTime = 1000.ToSeconds();

            return new(building, elevatorLogic, totalSimulationRunningTime, generator.Generate(60, floors, totalSimulationRunningTime));
        }

        #region Commands

        #region SimulationStep

        public ICommand StepCommand => _stepCommand ??= new CommandHandler(Step, CanStep);
        private ICommand? _stepCommand;

        private bool CanStep() => true;

        private void Step()
        {
            if (CanStep())
            {
                _Simulation.Step();
            }
        }

        #endregion

        #endregion
    }
}
