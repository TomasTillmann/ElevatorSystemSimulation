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
        private MainViewModel _DataContext => (MainViewModel)DataContext;

        private const double GroundLocation = 30;

        private const double FloorWidth = 10;
        private const double FloorHeight = 30;
        private const double VerticalSpaceBetweenFloors = 10;
        private const double HorizontalSpaceBetweenFloors = 10;

        private const double ElevatorWidth = 10;
        private const double ElevatorHeight = 30;
        private const double VerticalSpaceBetweenElevators = 10;
        private const double HorizontalSpaceBetweenElevators = 10;

        public IEnumerable<ElevatorView> Elevators { get => (IEnumerable<ElevatorView>)GetValue(ElevatorsProperty); set => SetValue(ElevatorsProperty, value); }
        public static readonly DependencyProperty ElevatorsProperty = DependencyProperty.Register("Elevators", typeof(IEnumerable<ElevatorView>), typeof(MainView));

        public IEnumerable<FloorView> Floors { get => (IEnumerable<FloorView>)GetValue(FloorsProperty); set => SetValue(FloorsProperty, value); }
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register("Floors", typeof(IEnumerable<FloorView>), typeof(MainView));

        public MainView()
        {
            InitializeComponent();

            InitFloors();
            DrawFloors();

            InitElevators();
        }

        private void DrawFloors()
        {
            double horizontalShift = GroundLocation;
            foreach(FloorView floor in Floors)
            {
                canvas.Children.Add(floor);
                Canvas.SetBottom(floor, horizontalShift);
                Canvas.SetLeft(floor, 10);
                horizontalShift += floor.Height + HorizontalSpaceBetweenFloors;
            }
        }

        private void InitFloors()
        {
            Floors = _DataContext
            .Floors
            .Select(f => new FloorView());
        }

        private void InitElevators()
        {
            Elevators = _DataContext
            .Elevators
            .Select(e => new ElevatorView());
        }
    }
}
