﻿using ElevatorSystemSimulation;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const double WallMargin = 10;

        private List<ElevatorView> _ElevatorViews;
        private List<FloorView> _FloorViews;

        public List<ElevatorViewModel> Elevators { get => (List<ElevatorViewModel>)GetValue(ElevatorsProperty); set => SetValue(ElevatorsProperty, value); }
        public static readonly DependencyProperty ElevatorsProperty = DependencyProperty.Register("Elevators", typeof(List<ElevatorViewModel>), typeof(BuildingView), new FrameworkPropertyMetadata(null, OnFloorsOrElevatorsChanged));

        public List<FloorViewModel> Floors { get => (List<FloorViewModel>)GetValue(FloorsProperty); set => SetValue(FloorsProperty, value); }
        public static readonly DependencyProperty FloorsProperty = DependencyProperty.Register("Floors", typeof(List<FloorViewModel>), typeof(BuildingView), new FrameworkPropertyMetadata(null, OnFloorsOrElevatorsChanged));

        public double Scale { get { return (double)GetValue(ScaleProperty); } set { SetValue(ScaleProperty, value); } }
        public static readonly  DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(BuildingView), new PropertyMetadata(1.0));

        public double ElevatorViewWidth { get { return (double)GetValue(ElevatorViewWidthProperty); } set { SetValue(ElevatorViewWidthProperty, value); } }
        public static readonly DependencyProperty ElevatorViewWidthProperty = DependencyProperty.Register("ElevatorViewWidth", typeof(double), typeof(BuildingView), new PropertyMetadata(10.0));

        public double ElevatorViewHeight { get { return (double)GetValue(ElevatorViewHeightProperty); } set { SetValue(ElevatorViewHeightProperty, value); } }
        public static readonly DependencyProperty ElevatorViewHeightProperty = DependencyProperty.Register("ElevatorViewHeight", typeof(double), typeof(BuildingView), new PropertyMetadata(30.0));

        public Brush BuildingBackground { get { return (Brush)GetValue(BuildingBackgroundProperty); } set { SetValue(BuildingBackgroundProperty, value); } }
        public static readonly DependencyProperty BuildingBackgroundProperty = DependencyProperty.Register("BuildingBackground", typeof(Brush), typeof(BuildingView));

        public double BuildingVerticalLocation { get { return (double)GetValue(BuildingVerticalLocationProperty); } set { SetValue(BuildingVerticalLocationProperty, value); } }
        public static readonly DependencyProperty BuildingVerticalLocationProperty = DependencyProperty.Register("BuildingVerticalLocation", typeof(double), typeof(BuildingView));

        public double BuildingHorizontalLocation { get { return (double)GetValue(BuildingHorizontalLocationProperty); } set { SetValue(BuildingHorizontalLocationProperty, value); } }
        public static readonly DependencyProperty BuildingHorizontalLocationProperty = DependencyProperty.Register("BuildingHorizontalLocation", typeof(double), typeof(BuildingView));

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
                    Height = ElevatorViewHeight,
                    PeopleCount = e.PeopleCount
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
            DrawRequests();
            DrawFloorSeparators();
            DrawGround();
        }

        private void DrawBuilding()
        {
            building.Width = WallMargin + _ElevatorViews.Count * (ElevatorViewWidth + WallMargin);
            building.Height = WallMargin + _FloorViews.Count * (ElevatorViewHeight + WallMargin);

            surroundings.Width = building.Width + 500;
            surroundings.Height = building.Height + 20;

            BuildingVerticalLocation = surroundings.Width / 2 - building.Width / 2;
            BuildingHorizontalLocation = 2 * WallMargin;


            if (Background == null)
            {
                Background = new SolidColorBrush(Colors.DarkGray);
            }

            double verticalPosition = WallMargin / 3;
            for(int i = 0; i < Elevators.Count + 1; i++)
            {
                Rectangle elevatorsDividerView = new()
                {
                    Fill = new SolidColorBrush(Color.FromRgb(155, 155, 158)),
                    Height = building.Height,
                    Width = WallMargin / 3,
                };
                Panel.SetZIndex(elevatorsDividerView, -1);

                building.Children.Add(elevatorsDividerView);
                Canvas.SetBottom(elevatorsDividerView, 0);
                Canvas.SetLeft(elevatorsDividerView, verticalPosition);
                verticalPosition += ElevatorViewWidth + WallMargin;
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

        private void DrawRequests()
        {
            double horizontalPosition = BuildingHorizontalLocation; 
            for(int i = 0; i < Floors.Count; i++)
            {
                RequestView onFloorRequestsView = new()
                {
                    Text = Floors[i].Requests.Count.ToString(),
                    TextHeight = ElevatorViewHeight,
                    TextWidth = ElevatorViewWidth,
                    FloorHeight = WallMargin,
                    FloorWidth = ElevatorViewWidth + WallMargin
                };

                surroundings.Children.Add(onFloorRequestsView);
                Canvas.SetLeft(onFloorRequestsView, BuildingVerticalLocation + building.Width);
                Canvas.SetBottom(onFloorRequestsView, horizontalPosition);
                Panel.SetZIndex(onFloorRequestsView, -1);
                horizontalPosition += ElevatorViewHeight + WallMargin;
            }
        }

        private void DrawFloorSeparators()
        {
            double floorSeparatorPosition = 0;
            for(int i = 0; i <= Floors.Count; i++)
            {
                Rectangle floorSeparatorView = new()
                {
                    Fill = new SolidColorBrush(Colors.DarkGray),
                    Height = WallMargin,
                    Width = building.Width
                };

                building.Children.Add(floorSeparatorView);
                Canvas.SetBottom(floorSeparatorView, floorSeparatorPosition);
                floorSeparatorPosition += ElevatorViewHeight + WallMargin;
            }
        }

        private void DrawGround()
        {
            Rectangle ground = new()
            {
                Fill = new SolidColorBrush(Colors.SandyBrown),
                Height = 2 * WallMargin,
                Width=surroundings.Width
            };

            surroundings.Children.Add(ground);
            Canvas.SetBottom(ground, 0);
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

    #region Converters
    public class DividerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double numericValue && (parameter is double numericParameter || Double.TryParse(parameter.ToString(), out numericParameter)))
            {
                return numericValue / numericParameter;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double numericValue && (parameter is double numericParameter || Double.TryParse(parameter.ToString(), out numericParameter)))
            {
                return numericValue * numericParameter;
            }

            return value;
        }
    }

    public class GetLeftLocationFromCenterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values[0] is double n1Value && values[1] is double n2Value)
            {
                return n1Value / 2 - n2Value / 2;
            }

            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
