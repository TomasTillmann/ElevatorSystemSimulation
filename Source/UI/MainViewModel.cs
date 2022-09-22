using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UI
{
    public class MainViewModel : DependencyObject
    {
        // Model
        private Simulation _Simulation { get; }

        public List<ElevatorViewModel> Elevators { get => (List<ElevatorViewModel>)GetValue(ElevatorsProperty); set => SetValue(ElevatorsProperty, value); }
        public static readonly DependencyProperty ElevatorsProperty = DependencyProperty.Register("Elevators", typeof(List<ElevatorViewModel>), typeof(MainViewModel));

        public List<FloorViewModel> Floors { get => (List<FloorViewModel>)GetValue(FloorsProperty); set => SetValue(FloorsProperty, value); }
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register("Floors", typeof(List<FloorViewModel>), typeof(MainViewModel));

        public List<SettingItem> Settings { get => (List<SettingItem>)GetValue(SettingsProperty); set => SetValue(SettingsProperty, value); }
        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("Settings", typeof(List<SettingItem>), typeof(MainViewModel));

        public double BuildingScale { get { return (double)GetValue(BuildingScaleProperty); } set { SetValue(BuildingScaleProperty, value); } }
        public static readonly DependencyProperty BuildingScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(MainViewModel), new PropertyMetadata(0.5));

        public int StepCount { get { return (int)GetValue(StepCountProperty); } set { SetValue(StepCountProperty, value); } }
        public static readonly DependencyProperty StepCountProperty = DependencyProperty.Register("StepCount", typeof(int), typeof(MainViewModel));

        public MainViewModel()
        {
            _Simulation = GetSimulation();

            Elevators = _Simulation.Building.ElevatorSystem.Elevators.Select(e => new ElevatorViewModel(e)).ToList();
            Floors = _Simulation.Building.Floors.Value.Select(f => new FloorViewModel(f)).ToList();
        }

        public void Step()
        {
            _Simulation.Step();
            StepCount = _Simulation.StepCount;

            UpdateElevatorViewModels();
            UpdateFloorViewModels();
        }

        public void Restart()
        {
            _Simulation.Restart();
            StepCount = 0;
            UpdateElevatorViewModels();
            UpdateFloorViewModels();
        }

        private Simulation GetSimulation()
        {
            // Mock Data
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
                    //new Floor(10, 250.ToCentimeters()),
                    //new Floor(11, 250.ToCentimeters()),
                    //new Floor(12, 250.ToCentimeters()),
                    //new Floor(13, 250.ToCentimeters()),
                    //new Floor(14, 250.ToCentimeters()),
                    //new Floor(15, 250.ToCentimeters()),
                    //new Floor(16, 250.ToCentimeters()),
                    //new Floor(17, 250.ToCentimeters()),
                    //new Floor(18, 250.ToCentimeters()),
                    //new Floor(19, 250.ToCentimeters()),
                    //new Floor(20, 250.ToCentimeters()),
                    //new Floor(21, 250.ToCentimeters()),
                    //new Floor(22, 250.ToCentimeters()),
                    //new Floor(23, 250.ToCentimeters()),
                    //new Floor(24, 250.ToCentimeters()),
                    //new Floor(25, 250.ToCentimeters()),
                    //new Floor(26, 250.ToCentimeters()),
                    //new Floor(27, 250.ToCentimeters()),
                    //new Floor(28, 250.ToCentimeters()),
                },
                10.ToCentimeters()
            );

            ElevatorSystem elevatorSystem = new ElevatorSystem(
                new List<Elevator>()
                {
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(0)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(14)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                    new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                    //new Elevator(100.ToCmPerSec(), 5.ToCmPerSec(), 10.ToSeconds(), 10, floors.GetFloorById(2)),
                }
            );

            Building building = new(floors, elevatorSystem);
            ClientsElevatorLogic elevatorLogic = new(building);
            ClientsRequestGenerator generator = new(new Random());
            Seconds totalSimulationRunningTime = 1000.ToSeconds();

            return new(building, elevatorLogic, totalSimulationRunningTime, generator.Generate(60, floors, totalSimulationRunningTime));
            //
        }

        private void UpdateElevatorViewModels()
        {
            int i = 0;
            foreach(ElevatorViewModel elevatorViewModel in Elevators)
            {
                Elevator elevator = _Simulation.Building.ElevatorSystem.Elevators[i++];
                elevatorViewModel.Location = elevator.Location;
                elevatorViewModel.PeopleCount = elevator.AttendingRequests.Count;
            }
        }

        private void UpdateFloorViewModels()
        {
            int i = 0;
            foreach(FloorViewModel floorViewModel in Floors)
            {
                Floor floor = _Simulation.Building.Floors.Value[i++];
                floorViewModel.Requests = floor.Requests;
            }
        }

        public void Step(object sender, RoutedEventArgs e)
        {
            Step();
        }

        public void Restart(object sender, RoutedEventArgs e)
        {
            Restart();
        }
    }

    public class SettingItem
    {

    }

    #region Converters

    public class AdderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double numericValue && (parameter is double numericParameter || Double.TryParse(parameter.ToString(), out numericParameter)))
            {
                return numericValue + numericParameter;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double numericValue && (parameter is double numericParameter || Double.TryParse(parameter.ToString(), out numericParameter)))
            {
                return numericValue - numericParameter;
            }

            return value;
        }
    }
    #endregion
}
