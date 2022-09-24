using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;
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
        //

        #region History

        private readonly List<SimulationSnapshot> _History = new();
        private int _CurrentSnapshotPointer = -1;

        #endregion

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

        public IEvent? LastEvent { get { return (IEvent)GetValue(LastEventProperty); } set { SetValue(LastEventProperty, value); } }
        public static readonly DependencyProperty LastEventProperty = DependencyProperty.Register("LastEvent", typeof(IEvent), typeof(MainViewModel));

        public MainViewModel()
        {
            _Simulation = GetSimulation();

            Elevators = _Simulation.Building.ElevatorSystem.Elevators.Select(e => new ElevatorViewModel(e)).ToList();
            Floors = _Simulation.Building.Floors.Value.Select(f => new FloorViewModel(f)).ToList();

            _History.Add(GetSimulationSnapshot());
            _CurrentSnapshotPointer++;
        }

        public void Step()
        {
            if(_CurrentSnapshotPointer == _History.Count - 1)
            {
                _Simulation.Step();
                UpdateViewModel();

                _History.Add(GetSimulationSnapshot());
                _CurrentSnapshotPointer++;
            }
            else
            {
                _CurrentSnapshotPointer++;
                UpdateViewModelFromSnapshot();
            }
        }

        public void StepBack()
        {
            _CurrentSnapshotPointer = _CurrentSnapshotPointer == 0 ? _CurrentSnapshotPointer : _CurrentSnapshotPointer - 1;
            UpdateViewModelFromSnapshot();
        }

        public void Restart()
        {
            _Simulation.Restart();

            UpdateViewModel();

            _History.Clear();
            _History.Add(GetSimulationSnapshot());
            _CurrentSnapshotPointer = 0;

            UpdateElevatorViewModels();
            UpdateFloorViewModels();
        }

        private Simulation GetSimulation()
        {
            return SimulationProvider.GetSimulation();
        }

        private void UpdateViewModel()
        {
            StepCount = _Simulation.StepCount;
            CurrentTime = _Simulation.CurrentTime.Value;
            LastEvent = _Simulation.LastEvent;

            UpdateElevatorViewModels();
            UpdateFloorViewModels();
        }

        private void UpdateElevatorViewModels()
        {
            int i = 0;
            foreach(ElevatorViewModel elevatorViewModel in Elevators)
            {
                Elevator elevatorModel = _Simulation.Building.ElevatorSystem.Elevators[i++];
                elevatorViewModel.Location = elevatorModel.Location;
                elevatorViewModel.PeopleCount = elevatorModel.AttendingRequests.Count;
            }
        }

        private void UpdateFloorViewModels()
        {
            int i = 0;
            foreach(FloorViewModel floorViewModel in Floors)
            {
                Floor floor = _Simulation.Building.Floors.Value[i++];
                floorViewModel.Requests = (List<IRequestEvent>)floor.Requests;
            }
        }

        private void UpdateViewModelFromSnapshot()
        {
            SimulationSnapshot currentSnapshot = _History[_CurrentSnapshotPointer];

            StepCount = currentSnapshot.StepCount;
            CurrentTime = currentSnapshot.CurrentTime;
            LastEvent = currentSnapshot.LastEvent;

            UpdateElevatorViewModelsFromSnapshot();
            UpdateFloorViewModelsFromSnapshot();
        }

        private void UpdateElevatorViewModelsFromSnapshot()
        {
            SimulationSnapshot currentSnapshot = _History[_CurrentSnapshotPointer];

            int i = 0;
            foreach (ElevatorViewModel elevatorViewModel in Elevators)
            {
                ElevatorSnapshot elevatorSnapshot = currentSnapshot.ElevatorSnapshots[i++];
                elevatorViewModel.Location = elevatorSnapshot.Location;
                elevatorViewModel.PeopleCount = elevatorSnapshot.PeopleCount;
            }
        }

        private void UpdateFloorViewModelsFromSnapshot()
        {
            SimulationSnapshot currentSnapshot = _History[_CurrentSnapshotPointer];

            int i = 0;
            foreach(FloorViewModel floorViewModel in Floors)
            {
                FloorSnapshot floorSnapshot = currentSnapshot.FloorSnapshots[i++];
                floorViewModel.Requests = floorSnapshot.RequestSnapshots.Select(r => (IRequestEvent)r).ToList();        //HACK - not very clean
            }
        }

        #region Listeners

        public void Step(object sender, RoutedEventArgs e)
        {
            Step();
        }

        public void StepBack(object sender, RoutedEventArgs e)
        {
            StepBack();
        }

        public void Restart(object sender, RoutedEventArgs e)
        {
            Restart();
        }

        #endregion

        #region History

        private SimulationSnapshot GetSimulationSnapshot()
        {
            List<ElevatorSnapshot> elevatorSnapshots =
                Elevators
                .Select(e => new ElevatorSnapshot(e.PeopleCount, e.Location)).ToList();

            List<FloorSnapshot> floorSnapshots =
                Floors
                .Select(f => new FloorSnapshot(
                    f.Requests
                    .Select(r => (BasicRequestEvent)r)          //HACK - not very clean - Could be some different requestEvent in the future
                    .ToList()))
                .ToList();

            return new SimulationSnapshot(StepCount, CurrentTime, LastEvent, elevatorSnapshots, floorSnapshots);
        }

        private struct SimulationSnapshot
        {
            public int StepCount { get; }
            public int CurrentTime { get; }
            public IEvent? LastEvent { get; }
            public List<ElevatorSnapshot> ElevatorSnapshots { get; }
            public List<FloorSnapshot> FloorSnapshots { get; }

            public SimulationSnapshot(int stepCount, int currentTime, IEvent? lastEvent, List<ElevatorSnapshot> elevatorSnapshots, List<FloorSnapshot> floorSnapshots)
            {
                StepCount = stepCount;
                CurrentTime = currentTime;
                LastEvent = lastEvent;
                ElevatorSnapshots = elevatorSnapshots;
                FloorSnapshots = floorSnapshots;
            }
        }

        private struct ElevatorSnapshot
        {
            public int PeopleCount { get; }
            public Centimeters Location { get; }

            public ElevatorSnapshot(int peopleCount, Centimeters location)
            {
                PeopleCount = peopleCount;
                Location = location;
            }
        }

        private struct FloorSnapshot
        {
            public List<BasicRequestEvent> RequestSnapshots { get; }

            public FloorSnapshot(List<BasicRequestEvent> requestSnapshots)
            {
                RequestSnapshots = requestSnapshots;
            }
        }

        #endregion
    }

    #region Helpers

    public class EventTemplateSelector : DataTemplateSelector 
    {
        public DataTemplate? ElevatorEventTemplate { get; set; }
        public DataTemplate? BasicRequestEventTemplate { get; set; } 
        public DataTemplate? NoEventTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if(item is ElevatorEvent)
            {
                return ElevatorEventTemplate;
            }

            if(item is BasicRequestEvent)
            {
                return BasicRequestEventTemplate;
            }

            if(item is null)
            {
                return NoEventTemplate;
            }

            return null;
        }
    }

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
