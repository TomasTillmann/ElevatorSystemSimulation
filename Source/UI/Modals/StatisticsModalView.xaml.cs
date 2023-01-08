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
using System.Windows.Shapes;

namespace UI
{
    /// <summary>
    /// Interaction logic for StatisticsView.xaml
    /// </summary>
    public partial class StatisticsModalView : Window
    {
        public StatisticsModalViewModel ViewModel => (StatisticsModalViewModel)DataContext;

        public StatisticsResult Stats { get { return (StatisticsResult)GetValue(StatsProperty); } set { SetValue(StatsProperty, value); } }
        public static readonly DependencyProperty StatsProperty = DependencyProperty.Register("StatsProperty", typeof(StatisticsResult), typeof(StatisticsModalView));

        public StatisticsModalView(Window owner, StatisticsResult stats)
        {
            Owner = owner;
            Owner.Opacity = 0.5;
            Stats = stats;

            InitializeComponent();
        }
    }
}
