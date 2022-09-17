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
    public partial class BuildingView : UserControl
    {
        private const int WallMargin = 10;

        private List<ElevatorView> _ElevatorViews;
        private List<FloorView> _FloorViews;

        public List<ElevatorViewModel> Elevators { get => (List<ElevatorViewModel>)GetValue(ElevatorsProperty); set => SetValue(ElevatorsProperty, value); }
        public static readonly DependencyProperty ElevatorsProperty = DependencyProperty.Register("Elevators", typeof(List<ElevatorViewModel>), typeof(BuildingView), new FrameworkPropertyMetadata(null, OnFloorsOrElevatorsChanged));

        public List<FloorViewModel> Floors { get => (List<FloorViewModel>)GetValue(FloorsProperty); set => SetValue(FloorsProperty, value); }
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register("Floors", typeof(List<FloorViewModel>), typeof(BuildingView), new FrameworkPropertyMetadata(null, OnFloorsOrElevatorsChanged));

        public double Scale { get { return (double)GetValue(ScaleProperty); } set { SetValue(ScaleProperty, value); } }
        public static readonly  DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(BuildingView), new PropertyMetadata(1.0));

        public double ElevatorViewWidth { get { return (double)GetValue(ElevatorWidthProperty); } set { SetValue(ElevatorWidthProperty, value); } }
        public static readonly DependencyProperty ElevatorWidthProperty = DependencyProperty.Register("ElevatorWidth", typeof(double), typeof(BuildingView), new PropertyMetadata(10.0));

        public double ElevatorViewHeight { get { return (double)GetValue(ElevatorHeightProperty); } set { SetValue(ElevatorHeightProperty, value); } }
        public static readonly DependencyProperty ElevatorHeightProperty = DependencyProperty.Register("ElevatorHeight", typeof(double), typeof(BuildingView), new PropertyMetadata(30.0));

        public BuildingView()
        {
            InitializeComponent();
            InitBuilding();

            PreviewMouseWheel += OnPreviewMouseWheelMoving; 
        }

        public void InitBuilding()
        {
            if(Elevators == null || Floors == null)
            {
                return;
            }

            _ElevatorViews =
                Elevators
                .Select(e => new ElevatorView
                {
                    Width = ElevatorViewWidth,
                    Height = ElevatorViewHeight
                })
                .ToList();

            _FloorViews =
                Floors
                .Select(f => new FloorView
                {
                    Width = ElevatorViewWidth + WallMargin,
                    Height = ElevatorViewHeight + WallMargin
                })
                .ToList();

            DrawBuilding();
            DrawElevators();
            DrawFloorSeparators();
        }

        private void DrawBuilding()
        {
            building.Width = WallMargin + _ElevatorViews.Count * (ElevatorViewWidth + WallMargin);
            building.Height = WallMargin + _FloorViews.Count * (ElevatorViewHeight + WallMargin);

            if(Background == null)
            {
                Background = new SolidColorBrush(Colors.DarkGray);
            }
        }

        private void DrawElevators()
        {
            double horizontalPosition = 0;
            for(int i = 0; i < Elevators.Count; i++)
            {
                ElevatorView elevatorView = _ElevatorViews[i];
                ElevatorViewModel elevatorViewModel = Elevators[i];

                building.Children.Add(elevatorView);
                Canvas.SetBottom(elevatorView, GetElevatorsViewVerticalLocation(elevatorViewModel, Floors[0].Height)); //TODO - get floor height better way - for now all floors have the same height
                Canvas.SetLeft(elevatorView, WallMargin + horizontalPosition);
                horizontalPosition += ElevatorViewWidth + WallMargin;
            }
        }

        private void DrawFloorSeparators()
        {
            double floorSeparatorPosition = 0;
            for(int i = 0; i <= Floors.Count; i++)
            {
                Rectangle floorSeparator = new()
                {
                    Fill = new SolidColorBrush(Colors.DarkGray),
                    Height = WallMargin,
                    Width = building.Width
                };

                building.Children.Add(floorSeparator);
                Canvas.SetBottom(floorSeparator, floorSeparatorPosition);
                floorSeparatorPosition += ElevatorViewHeight + WallMargin;
            }
        }

        private double GetElevatorsViewVerticalLocation(ElevatorViewModel elevatorViewModel, Centimeters floorHeight)
        {
            int floor = (elevatorViewModel.Location.Value / floorHeight.Value);
            double inFloorRatio = (double)(elevatorViewModel.Location.Value % floorHeight.Value) / floorHeight.Value;

            return WallMargin + floor * (WallMargin + ElevatorViewHeight) + inFloorRatio * ElevatorViewHeight;
        }

        private void OnPreviewMouseWheelMoving(object sender, MouseWheelEventArgs e)
        {
            if(sender is BuildingView buildingView)
            {
                if (Keyboard.Modifiers != ModifierKeys.Control)
                    return;

                if (e.Delta < 0)
                {
                    if(Scale >= 0.2)
                    {
                        double scale = Scale - 0.1;
                        if(scale >= 0.01)
                        {
                            Scale = scale;
                        }
                    }
                    else
                    {
                        double scale = Scale - 0.05;
                        if(scale >= 0.01)
                        {
                            Scale = scale;
                        }
                    }

                }
                else if(e.Delta > 0)
                {
                    Scale += 0.1;
                }
            }
        }

        private static void OnFloorsOrElevatorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is BuildingView buildingView)
            {
                buildingView.InitBuilding();
            }
        }
    }
}
