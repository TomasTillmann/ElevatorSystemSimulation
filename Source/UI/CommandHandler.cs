using System;
using System.Windows.Input;

namespace UI
{
    public interface ICommand<ParamterType> : ICommand
    {
        bool CanExecute(ParamterType parameter);
        void Execute(ParamterType parameter); 
    }

    public class CommandHandler<ParameterType> : ICommand<ParameterType>
    {
        private Action<ParameterType?> _Execute;
        private Func<ParameterType?, bool> _CanExecute;

        public CommandHandler(Action<ParameterType?> action, Func<ParameterType?, bool>? canExecute = null)
        {
            _Execute = action;
            _CanExecute =  canExecute ?? new Func<ParameterType?, bool>(_ => true);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        bool ICommand.CanExecute(object? parameter)
        {
            if(parameter == null)
            {
                return CanExecute(default(ParameterType));
            }

            return CanExecute((ParameterType)parameter);
        }

        public bool CanExecute(ParameterType? parameter)
        {
            return _CanExecute.Invoke(parameter);
        }

        void ICommand.Execute(object? parameter)
        {
            if(parameter == null)
            {
                Execute(default(ParameterType));
            }

            Execute((ParameterType)parameter);
        }

        public void Execute(ParameterType? parameter)
        {
            _Execute.Invoke(parameter);
        }
    }

    public class CommandHandler : ICommand
    {
        private Action<object?> _Execute;
        private Func<object?, bool> _CanExecute;

        public event EventHandler? CanExecuteChanged;

        public CommandHandler(Action<object?> action, Func<object?, bool>? canExecute = null)
        {
            _Execute = action;
            _CanExecute = canExecute ?? new Func<object?, bool>(_ => true);
        }

        public bool CanExecute(object? parameter)
        {
            return _CanExecute.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            _Execute.Invoke(parameter);
        }
    }
}
