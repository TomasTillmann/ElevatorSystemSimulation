using ElevatorSystemSimulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI
{
    public partial class MainView : UserControl
    {
        public new MainViewModel DataContext => (MainViewModel)base.DataContext;

        public MainView()
        {
            InitializeComponent();

            stepButton.Click += DataContext.Step;
            stepButton.Click += buildingView.Update;

            restartButton.Click += DataContext.Restart;
            restartButton.Click += buildingView.Update;
        }
    }
}
