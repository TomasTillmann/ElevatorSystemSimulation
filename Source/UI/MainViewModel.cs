using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using Client;
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

        public double BuildingScale { get { return (double)GetValue(BuildingScaleProperty); } set { SetValue(BuildingScaleProperty, value); } }
        public static readonly DependencyProperty BuildingScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(MainViewModel), new PropertyMetadata(0.5));

        public int StepCount { get { return (int)GetValue(StepCountProperty); } set { SetValue(StepCountProperty, value); } }
        public static readonly DependencyProperty StepCountProperty = DependencyProperty.Register("StepCount", typeof(int), typeof(MainViewModel));

        public int CurrentTime { get { return (int)GetValue(CurrentTimeProperty); } set { SetValue(CurrentTimeProperty, value); } }
        public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(int), typeof(MainViewModel));

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
            CurrentTime = _Simulation.CurrentTime.Value;

            UpdateElevatorViewModels();
            UpdateFloorViewModels();
        }

        public void Restart()
        {
            _Simulation.Restart();
            StepCount = 0;
            CurrentTime = 0;

            UpdateElevatorViewModels();
            UpdateFloorViewModels();
        }

        private Simulation GetSimulation()
        {
            return SimulationProvider.GetSimulation();
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
