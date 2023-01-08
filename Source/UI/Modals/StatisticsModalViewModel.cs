using System.Windows.Input;
using System.Windows;

namespace UI
{
    public class StatisticsModalViewModel : ViewModelBase
    {
        private void OnWindowClosing(Window modalWindow)
        {
            modalWindow.Close();
        }

        #region Commands

        public ICommand BackCommand => _BackCommand ??= new CommandHandler<Window>(Back);
        private ICommand? _BackCommand;

        public void Back(Window modalWindow)
        {
            OnWindowClosing(modalWindow);
        }

        #endregion
    }
}
