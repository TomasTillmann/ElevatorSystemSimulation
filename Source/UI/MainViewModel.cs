﻿using ElevatorSystemSimulation;
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
        private ISimulation<BasicRequest> _Simulation { get; set; }
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

        public ObservableCollection<BasicRequest> ServedRequests  { get { return (ObservableCollection<BasicRequest>)GetValue(ServedRequestsProperty); } set { SetValue(ServedRequestsProperty, value); } }
        public static readonly DependencyProperty ServedRequestsProperty = DependencyProperty.Register("ServedRequests", typeof(ObservableCollection<BasicRequest>), typeof(MainViewModel));

        public ObservableCollection<BasicRequest> AllRequests  { get { return (ObservableCollection<BasicRequest>)GetValue(AllRequestsProperty); } set { SetValue(AllRequestsProperty, value); } }
        public static readonly DependencyProperty AllRequestsProperty = DependencyProperty.Register("AllRequestskk", typeof(ObservableCollection<BasicRequest>), typeof(MainViewModel));

        public IEventViewModel? LastEvent { get { return (IEventViewModel?)GetValue(LastEventProperty); } set { SetValue(LastEventProperty, value); } }
        public static readonly DependencyProperty LastEventProperty = DependencyProperty.Register("LastEvent", typeof(IEventViewModel), typeof(MainViewModel));

        public IEventViewModel? LastAction { get { return (IEventViewModel?)GetValue(LastActionProperty); } set { SetValue(LastActionProperty, value); } }
        public static readonly DependencyProperty LastActionProperty = DependencyProperty.Register("LastAction", typeof(IEventViewModel), typeof(MainViewModel));

        public MainViewModel()
        {
            InitSimulation(GetInitSimulation());

            Restart();
        }

        public void Step()
        {
            if(_CurrentSnapshotPointer == _History.Count - 1)
            {
                if (_Simulation.IsOver)
                {
                    return;
                }

                _Simulation.Step();
                UpdateThisViewModel();

                _History.Add(GetSnapshot());
                _CurrentSnapshotPointer++;
            }
            else
            {
                _CurrentSnapshotPointer++;
                UpdateThisViewModelFromSnapshot();
            }
        }

        public void StepBack()
        {
            _CurrentSnapshotPointer = _CurrentSnapshotPointer == 0 ? _CurrentSnapshotPointer : _CurrentSnapshotPointer - 1;
            UpdateThisViewModelFromSnapshot();
        }

        public void Restart()
        {
            _Simulation.Restart();

            UpdateThisViewModel();

            _History.Clear();
            _History.Add(GetSnapshot());
            _CurrentSnapshotPointer = 0;
        }

        private void InitSimulation(Simulation<BasicRequest> simulation)
        {
            _Simulation = simulation;

            Elevators = _Simulation.Building.ElevatorSystem.Elevators.Select(e => new ElevatorViewModel(e)).ToList();
            Floors = _Simulation.Building.Floors.Value.Select(f => new FloorViewModel(f)).ToList();

            UpdateRequests(simulation);
        }

        private void UpdateRequests(Simulation<BasicRequest> simulation)
        {
            if(ServedRequests is null)
            {
                ServedRequests = new ObservableCollection<BasicRequest>(_Simulation.DepartedRequests);
            }
            else
            {
                ServedRequests.Clear();
            }

            if(AllRequests is null)
            {
                AllRequests = new ObservableCollection<BasicRequest>(_Simulation.AllRequests);
            }
            else
            {
                //NOTE: Doing this to update the view. PropertyChanged event is not triggered by assignment only.
                foreach(BasicRequest req in _Simulation.AllRequests)
                {
                    AllRequests.Add(req);
                }
            }
        }

        private Simulation<BasicRequest> GetInitSimulation()
        {
            return SimulationProvider.GetSimulation();
        }

        private void UpdateThisViewModel()
        {
            StepCount = _Simulation.StepCount;
            CurrentTime = _Simulation.CurrentTime.Value;

            UpdateRequests(AllRequests, _Simulation.AllRequests);
            UpdateRequests(ServedRequests, _Simulation.DepartedRequests);

            UpdateLastEvent();
            UpdateLastAction();

            UpdateElevatorViewModels();
            UpdateFloorViewModels();
        }

        private void UpdateRequests(ICollection<BasicRequest> originalCollection, IReadOnlyList<BasicRequest> simulationCollection)
        {
            if(originalCollection.Count < simulationCollection.Count)
            {
                for(int i = originalCollection.Count; i < simulationCollection.Count; i++)
                {
                    originalCollection.Add(simulationCollection[i]);
                }
            }
            else if(originalCollection.Count != simulationCollection.Count)
            {
                originalCollection.Clear();
            }
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
                floorViewModel.Requests = (List<Request>)floor.Requests;
            }
        }

        private void UpdateLastEvent()
        {
            if(_Simulation.LastEvent is ElevatorEvent elevatorEvent)
            {
                LastEvent = new ElevatorEventViewModel(elevatorEvent);
            }
            else if (_Simulation.LastEvent is BasicRequest requestEvent)
            {
                LastEvent = new BasicRequestEventViewModel(requestEvent);
            }
            else
            {
                LastEvent = null;
            }

        }

        private void UpdateLastAction()
        {
            if(_Simulation.LastAction is ElevatorEvent elevatorEvent)
            {
                LastAction = new ElevatorEventViewModel(elevatorEvent);
            }
            else if (_Simulation.LastAction is BasicRequest requestEvent)
            {
                LastAction= new BasicRequestEventViewModel(requestEvent);
            }
            else
            {
                LastAction = null;
            }
        }

        #region Commands

        public ICommand ShowStatisticsCommand => _ShowStatisticsCommand ??= new CommandHandler<Window>(ShowStatistics, CanShowStatistics);
        private ICommand _ShowStatisticsCommand;

        private bool CanShowStatistics(Window _)
        {
            return true; 
        }

        private void ShowStatistics(Window owner)
        {
            if (!CanShowStatistics(owner))
            {
                return;
            }

            StatisticsModalView statisticsView = new(owner, _Simulation.GetStatistics(), _Simulation);
            statisticsView.ShowDialog();

            owner.Opacity = 1;
        }

        public ICommand ShowModalElevatorSystemPickerCommand => _ShowModalElevatorSystemPickerCommand ??= new CommandHandler<Window>(ShowModalElevatorSystemPicker);
        private ICommand _ShowModalElevatorSystemPickerCommand;

        private void ShowModalElevatorSystemPicker(Window owner)
        {
            MenuModalView elevatorSystemPicker = new(owner, _Simulation.Building);

            bool? result = elevatorSystemPicker.ShowDialog();

            if (result == null)
            {

            }
            else if (result.Value)
            {
                if(elevatorSystemPicker.ResultingSimulation is not null)
                {
                    InitSimulation(elevatorSystemPicker.ResultingSimulation);
                    Restart();
                }
                else
                {
                    throw new Exception("SHOULDNT HAPPEN - modal window view model should validate before saving if its not null or empty.");
                }
            }
        }

        #endregion

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

        private SimulationSnapshot GetSnapshot()
        {
            List<ElevatorViewModel> elevatorSnapshots = new(Elevators.Count);
            Elevators.ForEach(elevator => elevatorSnapshots.Add(elevator.Clone()));

            List<FloorViewModel> floorSnapshots = new(Floors.Count);
            Floors.ForEach(floor => floorSnapshots.Add(floor.Clone()));

            return new SimulationSnapshot(StepCount, CurrentTime, LastEvent?.Clone() as IEventViewModel, LastAction?.Clone() as IEventViewModel, elevatorSnapshots, floorSnapshots);
        }

        private void UpdateThisViewModelFromSnapshot()
        {
            SimulationSnapshot currentSnapshot = _History[_CurrentSnapshotPointer];

            StepCount = currentSnapshot.StepCount;
            CurrentTime = currentSnapshot.CurrentTime;
            LastEvent = currentSnapshot.LastEvent;
            LastAction = currentSnapshot.LastAction;

            for (int i = 0; i < Elevators.Count; i++)
            {
                Elevators[i] = currentSnapshot.ElevatorSnapshots[i];
            }

            for (int i = 0; i < Floors.Count; i++)
            {
                Floors[i] = currentSnapshot.FloorSnapshots[i];
            }
        }

        #endregion
    }

    public struct SimulationSnapshot
    {
        public int StepCount { get; }
        public int CurrentTime { get; }
        public IEventViewModel? LastEvent { get; }
        public IEventViewModel? LastAction { get; }
        public List<ElevatorViewModel> ElevatorSnapshots { get; }
        public List<FloorViewModel> FloorSnapshots { get; }

        public SimulationSnapshot(int stepCount, int currentTime, IEventViewModel? lastEvent, IEventViewModel? lastAction, List<ElevatorViewModel> elevatorSnapshots, List<FloorViewModel> floorSnapshots)
        {
            StepCount = stepCount;
            CurrentTime = currentTime;
            LastEvent = lastEvent;
            LastAction = lastAction;
            ElevatorSnapshots = elevatorSnapshots;
            FloorSnapshots = floorSnapshots;
        }
    }

    #region Helpers

    public class EventTemplateSelector : DataTemplateSelector 
    {
        public DataTemplate? ElevatorEventTemplate { get; set; }
        public DataTemplate? BasicRequestEventTemplate { get; set; } 
        public DataTemplate? NoEventTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if(item is ElevatorEventViewModel)
            {
                return ElevatorEventTemplate;
            }

            if(item is BasicRequestEventViewModel)
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
    public class SubstracterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double numericValue && (parameter is double numericParameter || Double.TryParse(parameter.ToString(), out numericParameter)))
            {
                return numericValue - numericParameter;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double numericValue && (parameter is double numericParameter || Double.TryParse(parameter.ToString(), out numericParameter)))
            {
                return numericValue + numericParameter;
            }

            return value;
        }
    }

    public class ZeroToCollapsedConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int numericValue)
            {
                return numericValue == 0 ? Visibility.Collapsed : Visibility.Visible;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
