using System.Windows.Input;
using System.Windows;
using System.Security.AccessControl;
using System.IO;
using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using Client;

namespace UI
{
    public class StatisticsModalViewModel : ViewModelBase
    {
        const string exportpathbase = @"C:\Users\tomas\Desktop\ElevatorSystemSimulation\Stats\";
        private void OnWindowClosing(Window modalWindow)
        {
            modalWindow.Close();
        }

        #region Commands

        public ICommand ExportCommand => _ExportCommand ??= new CommandHandler<ExportInfo>(Export);
        private ICommand? _ExportCommand;

        public void Export(ExportInfo info)
        {
            File.WriteAllText(exportpathbase + info.ExportFileName + ".txt", info.Stats.Serialize());
        }

        public ICommand BackCommand => _BackCommand ??= new CommandHandler<Window>(Back);
        private ICommand? _BackCommand;

        public void Back(Window modalWindow)
        {
            OnWindowClosing(modalWindow);
        }

        #endregion
    }
}
