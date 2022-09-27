using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace UI
{
    public class ElevatorSystemPickerViewModel
    {

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

        public ICommand CancelCommand => _CancelCommand ??= new CommandHandler<Window>(Cancel, CanCancel);
        private ICommand? _CancelCommand;

        public void Cancel(Window? window)
        {
            if(window is null)
            {
                return;
            }

            window.Close();
        }

        public bool CanCancel(Window? _)
        {
            return true;
        }

        #endregion
    }
}
