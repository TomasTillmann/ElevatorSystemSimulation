using ElevatorSystemSimulation.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace UI
{
    public class ElevatorSystemPickerViewModel : ViewModelBase
    {
        public ObservableCollection<ElevatorViewModel> Elevators { get; } = new();
        public ObservableCollection<FloorViewModel> Floors { get; } = new();

        public int TotalSimulationTime { get { return (int)GetValue(TotalSimulationTimeProperty); } set { SetValue(TotalSimulationTimeProperty, value); } }
        public static readonly DependencyProperty TotalSimulationTimeProperty = DependencyProperty.Register("TotalSimulationTime", typeof(int), typeof(ElevatorSystemPickerViewModel));

        public IElevatorLogic Algorithm { get { return (IElevatorLogic)GetValue(AlgorithmProperty); } set { SetValue(AlgorithmProperty, value); } }
        public static readonly DependencyProperty AlgorithmProperty = DependencyProperty.Register("Algorithm", typeof(IElevatorLogic), typeof(ElevatorSystemPickerViewModel));

        #region Commands

        public ICommand SaveCommand => _SaveCommand ??= new CommandHandler(Save, CanSave);
        private ICommand? _SaveCommand;

        public void Save(object? _)
        {
        }

        public bool CanSave(object? _)
        {
            return true;
        }

        public ICommand CancelCommand => _CancelCommand ??= new CommandHandler(Cancel, CanCancel);
        private ICommand? _CancelCommand;

        public void Cancel(object? window)
        {
        }

        public bool CanCancel(object? _)
        {
            return true;
        }

        #endregion
    }
}
