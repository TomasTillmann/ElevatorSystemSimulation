﻿using Client;
using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;
using System;
using System.Buffers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace UI
{
    public class MenuModalViewModel : ViewModelBase
    {
        public ObservableCollection<ElevatorInModalViewModel> Elevators { get; set; } = new();

        public ElevatorInModalViewModel? SelectedElevator { get { return (ElevatorInModalViewModel)GetValue(SelectedElevatorProperty); } set { SetValue(SelectedElevatorProperty, value); } }
        public static readonly DependencyProperty SelectedElevatorProperty = DependencyProperty.Register("SelectedElevator", typeof(ElevatorInModalViewModel), typeof(MenuModalViewModel));

        public ObservableCollection<FloorInModalViewModel> Floors { get; set; } = new();

        public FloorInModalViewModel? SelectedFloor { get { return (FloorInModalViewModel)GetValue(SelectedFloorProperty); } set { SetValue(SelectedFloorProperty, value); } }
        public static readonly DependencyProperty SelectedFloorProperty = DependencyProperty.Register("SelectedFloor", typeof(FloorInModalViewModel), typeof(MenuModalViewModel));

        public ObservableCollection<AlgorithmInModalViewModel> Algorithms { get; set; } = new();

        public AlgorithmInModalViewModel SelectedAlgorithm { get { return (AlgorithmInModalViewModel)GetValue(SelectedAlgorithmProperty); } set { SetValue(SelectedAlgorithmProperty, value); } }
        public static readonly DependencyProperty SelectedAlgorithmProperty = DependencyProperty.Register("SelectedAlgorithm", typeof(AlgorithmInModalViewModel), typeof(MenuModalViewModel));

        public IElevatorLogic<BasicRequest> Algorithm { get { return (IElevatorLogic<BasicRequest>)GetValue(AlgorithmProperty); } set { SetValue(AlgorithmProperty, value); } }
        public static readonly DependencyProperty AlgorithmProperty = DependencyProperty.Register("Algorithm", typeof(IElevatorLogic<BasicRequest>), typeof(MenuModalViewModel));

        public ISimulation? ResultingSimulation { get { return (ISimulation?)GetValue(ResultingSimulationProperty); } set { SetValue(ResultingSimulationProperty, value); } }
        public static readonly DependencyProperty ResultingSimulationProperty = DependencyProperty.Register("ResultingSimulation", typeof(ISimulation), typeof(MenuModalViewModel));

        public int Seed { get { return (int)GetValue(SeedProperty); } set { SetValue(SeedProperty, value); } }
        public static readonly DependencyProperty SeedProperty = DependencyProperty.Register("Seed", typeof(int), typeof(MenuModalViewModel));

        public int RequestsCount { get { return (int)GetValue(RequestsCountProperty); } set { SetValue(RequestsCountProperty, value); } }
        public static readonly DependencyProperty RequestsCountProperty = DependencyProperty.Register("RequestsCount", typeof(int), typeof(MenuModalViewModel));
        public int RequestsTimeSpan { get { return (int)GetValue(RequestsTimeSpanProperty); } set { SetValue(RequestsTimeSpanProperty, value); } }
        public static readonly DependencyProperty RequestsTimeSpanProperty = DependencyProperty.Register("RequestsTimeSpan", typeof(int), typeof(MenuModalViewModel));

        public Window Owner;

        private void OnWindowClosing(Window modalWindow)
        {
            Owner.Opacity = 1;
            modalWindow.Close();
        }

        public MenuModalViewModel()
        {
            Algorithms.Add(new AlgorithmInModalViewModel(ElevatorLogicType.SCAN, "SCAN", "The most widely used"));
            Algorithms.Add(new AlgorithmInModalViewModel(ElevatorLogicType.Greedy, "Greedy", "Chooses always the closest elevators. Leads to starvation. Can't be used generally."));
            Algorithms.Add(new AlgorithmInModalViewModel(ElevatorLogicType.DestinationDispatch, "DestinationDispatch", "User Specifies destination before getting to the elevator. Can group people with the same destination then."));

            Select(Algorithms.FirstOrDefault());
        }

        #region Commands

        public ICommand SaveCommand => _SaveCommand ??= new CommandHandler<Window>(Save, CanSave);
        private ICommand? _SaveCommand;

        public void Save(Window modalWindow)
        {
            if (CanSave(modalWindow))
            {
                Floors floors = new(Floors.Select(f => f.ToFloor()).ToList(), Floors[0].Height.ToCentimeters());
                ElevatorSystem elevatorSystem = new (Elevators.Select(e => e.ToElevator()).ToList());
                Building building = new(floors, elevatorSystem, new Population(RequestsTimeSpan.ToSeconds(), RequestsCount, Seed));

                Algorithm = SelectedAlgorithm.ToAlgorithm(building);

                // TODO: user could choose distribution - add support for distributions, so far only uniform is allowed
                BasicRequestsGenerator generator = new(new Random(Seed));

                ResultingSimulation = new Simulation<BasicRequest>(building, Algorithm, generator.Generate(RequestsCount, floors, RequestsTimeSpan.ToSeconds()));

                modalWindow.DialogResult = true;
                OnWindowClosing(modalWindow);
            }
            else
            {
                MessageBox.Show("Cannot save!");
            }
        }

        public bool CanSave(object modalWindow)
        {
            return Elevators.Count != 0 && Floors.Count != 0 && Floors.All(f => f.Height > 0);
        }

        public ICommand CancelCommand => _CancelCommand ??= new CommandHandler<Window>(Cancel);
        private ICommand? _CancelCommand;

        public void Cancel(Window modalWindow)
        {
            // TODO - prompt the user if he really want to close without saves

            OnWindowClosing(modalWindow);
        }

        public ICommand AddCommand => _AddCommand ??= new CommandHandler<SelectionStatesInModalView>(Add);
        private ICommand? _AddCommand;

        public void Add(SelectionStatesInModalView toAdd)
        {
            if(toAdd == SelectionStatesInModalView.Elevators)
            {
                ElevatorInModalViewModel elevator = new ElevatorInModalViewModel();
                Elevators.Add(elevator);
                Select(elevator);
            }
            else if(toAdd == SelectionStatesInModalView.Floors)
            {
                FloorInModalViewModel floor = new FloorInModalViewModel();
                Floors.Add(floor);
                Select(floor);
            }
        }

        public ICommand SelectCommand => _SelectCommand ??= new CommandHandler(Select);
        private ICommand? _SelectCommand;

        public void Select(object? parameter)
        {
            if(parameter is ElevatorInModalViewModel elevator)
            {
                if(SelectedElevator is not null)
                {
                    SelectedElevator.IsSelected = false;
                }

                SelectedElevator = elevator;

                if(SelectedElevator is not null)
                {
                    SelectedElevator.IsSelected = true;
                }
            }
            else if(parameter is FloorInModalViewModel floor)
            {
                if(SelectedFloor is not null)
                {
                    SelectedFloor.IsSelected = false;
                }

                SelectedFloor = floor;

                if(SelectedFloor is not null)
                {
                    SelectedFloor.IsSelected = true;
                }
            }
            else if(parameter is AlgorithmInModalViewModel algorithm)
            {
                if(SelectedAlgorithm is not null)
                {
                    SelectedAlgorithm.IsSelected = false;
                }

                SelectedAlgorithm = algorithm;

                if(SelectedAlgorithm is not null)
                {
                    SelectedAlgorithm.IsSelected = true;
                }
            }
        }

        public ICommand DeleteCommand => _DeleteCommand ??= new CommandHandler(Delete);
        private ICommand? _DeleteCommand;

        public void Delete(object? parameter)
        {
            if(parameter is ElevatorInModalViewModel elevator)
            {
                SelectedElevator = null;
                Elevators.Remove(elevator);
                Select(Elevators.FirstOrDefault());
            }
            else if(parameter is FloorInModalViewModel floor)
            {
                SelectedFloor = null;
                Floors.Remove(floor);
                Select(Floors.FirstOrDefault());
            }
        }

        #endregion
    }

    public class ElevatorInModalViewModel : ViewModelBase<Elevator>, ISelectable
    {
        public int TravelSpeed { get { return (int)GetValue(TravelSpeedProperty); } set { SetValue(TravelSpeedProperty, value); } }
        public static readonly DependencyProperty TravelSpeedProperty = DependencyProperty.Register("TravelSpeed", typeof(int), typeof(ElevatorInModalViewModel));

        public int DepartingTime { get { return (int)GetValue(DepartingTimeProperty); } set { SetValue(DepartingTimeProperty, value); } }
        public static readonly DependencyProperty DepartingTimeProperty = DependencyProperty.Register("DepartingTime", typeof(int), typeof(ElevatorInModalViewModel));

        public int Capacity { get { return (int)GetValue(CapacityProperty); } set { SetValue(CapacityProperty, value); } }
        public static readonly DependencyProperty CapacityProperty = DependencyProperty.Register("Capacity", typeof(int), typeof(ElevatorInModalViewModel));

        public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(ElevatorInModalViewModel));

        public ElevatorInModalViewModel(Elevator model) : base(model)
        {
            TravelSpeed = model.TravelSpeed.Value;
            DepartingTime = model.DepartingTime.Value;
            Capacity = model.Capacity;
        }

        public ElevatorInModalViewModel() : base(null)
        {
            // default values
            TravelSpeed = 80;
            DepartingTime = 10;
            Capacity = 10;
        }

        public Elevator ToElevator()
        {
            return new Elevator(new CentimetersPerSecond(TravelSpeed), new CentimetersPerSecond(0), DepartingTime.ToSeconds(), Capacity);
        }
    }

    public class FloorInModalViewModel : ViewModelBase<Floor>, ISelectable
    {
        public int Height { get { return (int)GetValue(HeightProperty); } set { SetValue(HeightProperty, value); } }
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(int), typeof(FloorInModalViewModel));

        public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(FloorInModalViewModel));

        public FloorInModalViewModel(Floor model) : base(model)
        {
            Height = model.Height.Value;
        }

        public FloorInModalViewModel() : base(null)
        {
            // default value
            Height = 250;
        }

        public Floor ToFloor()
        {
            return new Floor(Height.ToCentimeters());
        }
    }

    public class AlgorithmInModalViewModel : ViewModelBase<ElevatorLogicType>, ISelectable
    {
        public string Name { get { return (string)GetValue(NameProperty); } set { SetValue(NameProperty, value); } }
        public static readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(AlgorithmInModalViewModel));
        public string Description { get { return (string)GetValue(DescriptionProperty); } set { SetValue(DescriptionProperty, value); } }
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(AlgorithmInModalViewModel));

        public bool IsSelected { get { return (bool)GetValue(IsSelectedProperty); } set { SetValue(IsSelectedProperty, value); } }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(AlgorithmInModalViewModel));

        public AlgorithmInModalViewModel(ElevatorLogicType elevatorLogicType, string name, string description) : base(elevatorLogicType)
        {
            Name = name;
            Description = description;
        }

        public IElevatorLogic<BasicRequest> ToAlgorithm(Building building)
        {
            switch (Model)
            {
                case ElevatorLogicType.SCAN:
                    return new SCAN(building);

                case ElevatorLogicType.Greedy:
                    return new Greedy(building);

                case ElevatorLogicType.DestinationDispatch:
                    return new DestinationDispatch(building);
            }

            return null;
        }
    }

    public enum ElevatorLogicType
    {
        SCAN,
        Greedy,
        DestinationDispatch,
    }

    #region Converters

    public class NullToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null)
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CmToMsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int ivalue)
            {
                return ivalue / 100d;
            }

            throw new ArgumentException("Converter used badly");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int ivalue)
            {
                return ivalue * 100;
            }

            if(value is double dvalue)
            {
                return (int)dvalue * 100;
            }

            if(value is string svalue)
            {
                return (int)(double.Parse(svalue, System.Globalization.CultureInfo.InvariantCulture) * 100);
            }

            throw new ArgumentException("Converter used badly");
        }
    }

    #endregion
}
