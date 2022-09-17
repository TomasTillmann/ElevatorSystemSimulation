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
    public partial class BuildingView : UserControl
    {
        public List<ElevatorViewModel> Elevators { get => (List<ElevatorViewModel>)GetValue(ElevatorsProperty); set => SetValue(ElevatorsProperty, value); }
        public static readonly DependencyProperty ElevatorsProperty = DependencyProperty.Register("Elevators", typeof(List<ElevatorViewModel>), typeof(BuildingView), new FrameworkPropertyMetadata(null, RenderBuilding));

        public List<FloorViewModel> Floors { get => (List<FloorViewModel>)GetValue(FloorsProperty); set => SetValue(FloorsProperty, value); }
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register("Floors", typeof(List<FloorViewModel>), typeof(BuildingView), new FrameworkPropertyMetadata(null, RenderBuilding));


        public BuildingView()
        {
            InitializeComponent();
        }

        private static void RenderBuilding(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BuildingView building)
            {

            }
        }
    }
}
