using System.Windows.Input;
using System.Windows;
using System.Security.AccessControl;
using System.IO;
using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
using Client;
using System;

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
            string file = exportpathbase + info.ExportFileName + ".txt";
            try
            {
                File.WriteAllText(file, info.Stats.Serialize());
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Export to {file} didn't work. Try checking if directory path exists, if you have enough permission and if the filename is in a correct format.", "\u274C Export Error");
            }
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
