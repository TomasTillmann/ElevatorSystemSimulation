﻿using Client;
using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
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
        private List<RequestView> _RequestViews = new();

        private const string BuildingUid = "building";
        private const string RequestUid = "request";
        private const string FloorSeparatorUid = "floorSeparator";
        private const string GroundUid = "ground";
        private const string FloorUid = "floor";
        private const string ElevatorDividerUid = "elevatorDivider";

        private readonly HashSet<string> DrawnElementsUidsTracker = new();

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

        public IEventViewModel? LastEvent { get { return (IEventViewModel?)GetValue(LastEventProperty); } set { SetValue(LastEventProperty, value); } }
        public static readonly DependencyProperty LastEventProperty = DependencyProperty.Register("LastEvent", typeof(IEventViewModel), typeof(BuildingView));

        public BuildingView()
        {
            InitializeComponent();
            Init();

            PreviewMouseWheel += OnPreviewMouseWheelMoving;
        }

        public void UpdateViewAfterStep()
        {
            UpdateElevators();
            UpdateRequests();
            ShowWhereIsLastEvent();
        }

        private void Init()
        {
            if(Elevators == null || Floors == null)
            {
                return;
            }

            DrawBuilding();
            DrawElevators();
            DrawRequests();
            DrawFloorSeparators();
            DrawFloorIds();
            DrawGround();
        }

        private void DrawBuilding()
        {
            building.Width = WallMargin + Elevators.Count * (ElevatorViewWidth + WallMargin);
            building.Height = WallMargin + Floors.Count * (ElevatorViewHeight + WallMargin);

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
                    Uid=$"{ElevatorDividerUid}{i}"
                };

                DrawnElementsUidsTracker.Add(elevatorsDividerView.Uid);

                Panel.SetZIndex(elevatorsDividerView, -1);

                building.Children.Add(elevatorsDividerView);
                Canvas.SetBottom(elevatorsDividerView, 0);
                Canvas.SetLeft(elevatorsDividerView, verticalPosition);
                verticalPosition += ElevatorViewWidth + WallMargin;
            }
        }

        private void DrawElevators()
        {
            _ElevatorViews =
                Elevators
                .Select(e =>
                {
                    DrawnElementsUidsTracker.Add(e.Id.ToString());
                    return new ElevatorView
                    {
                        Width = ElevatorViewWidth,
                        Height = ElevatorViewHeight,
                        PeopleCount = e.PeopleCount,
                        Uid = e.Id.ToString()
                    };
                })
                .ToList();

            double horizontalPosition = 0;
            for(int i = 0; i < Elevators.Count; i++)
            {
                ElevatorView elevatorView = _ElevatorViews[i];
                ElevatorViewModel elevatorViewModel = Elevators[i];

                building.Children.Add(elevatorView);
                Canvas.SetBottom(elevatorView, GetElevatorsViewVerticalLocation(elevatorViewModel, Floors[0].Height)); //TODO - get floor height better way - for now all floors have the same height
                Canvas.SetLeft(elevatorView, WallMargin + horizontalPosition);
                Panel.SetZIndex(elevatorView, 1000);
                horizontalPosition += ElevatorViewWidth + WallMargin;
            }
        }

        private void DrawRequests()
        {

            _RequestViews.Clear();

            double horizontalPosition = BuildingHorizontalLocation; 
            for(int i = 0; i < Floors.Count; i++)
            {
                RequestView requestViewOnFloor = new()
                {
                    Text = Floors[i].Requests.Count.ToString(),
                    TextHeight = ElevatorViewHeight,
                    TextWidth = ElevatorViewWidth,
                    FloorHeight = WallMargin,
                    FloorWidth = ElevatorViewWidth + WallMargin,
                    Uid=$"{RequestUid}{i}"
                };

                DrawnElementsUidsTracker.Add(requestViewOnFloor.Uid);

                surroundings.Children.Add(requestViewOnFloor);
                Canvas.SetLeft(requestViewOnFloor, BuildingVerticalLocation + building.Width);
                Canvas.SetBottom(requestViewOnFloor, horizontalPosition);
                Panel.SetZIndex(requestViewOnFloor, -1);
                horizontalPosition += ElevatorViewHeight + WallMargin;

                _RequestViews.Add(requestViewOnFloor);
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
                    Width = building.Width,
                    Uid=$"{FloorSeparatorUid}{i}"
                };

                DrawnElementsUidsTracker.Add(floorSeparatorView.Uid);

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
                Width=surroundings.Width,
                Uid=GroundUid
            };

            DrawnElementsUidsTracker.Add(ground.Uid);

            surroundings.Children.Add(ground);
            Canvas.SetBottom(ground, 0);
        }

        private void DrawFloorIds()
        {
            double verticalShift = 2 * WallMargin + ElevatorViewHeight / 2;
            for(int i = 0; i < Floors.Count; i++)
            {
                TextBox floorId = new()
                {
                    Text = Floors[i].Id.ToString(),
                    Background = new SolidColorBrush(Colors.Transparent),
                    FontSize = 20,
                    BorderBrush = new SolidColorBrush(Colors.Transparent),
                    Uid=$"{FloorUid}{i}"
                };

                DrawnElementsUidsTracker.Add(floorId.Uid);

                surroundings.Children.Add(floorId);
                Canvas.SetLeft(floorId, BuildingHorizontalLocation + (surroundings.Width - building.Width) / 2 - 6 * WallMargin);
                Canvas.SetBottom(floorId, verticalShift);
                verticalShift += ElevatorViewHeight + WallMargin;

            }
        }

        private void UpdateElevators()
        {
            int i = 0;
            foreach(ElevatorView elevatorView in _ElevatorViews)
            {
                ElevatorViewModel elevatorViewModel = Elevators[i++]; 
                Canvas.SetBottom(elevatorView, GetElevatorsViewVerticalLocation(elevatorViewModel, Floors[0].Height)); //TODO - get floor height better way - for now all floors have the same height

                elevatorView.PeopleCount = elevatorViewModel.PeopleCount;
                elevatorView.Background = ElevatorView.DefaultBackground;
            }
        }

        private void UpdateRequests()
        {
            int i = 0;
            foreach(RequestView request in _RequestViews)
            {
                request.Text = Floors[i++].Requests.Count.ToString();
                request.Background = RequestView.DefaultBackground;
            }
        }

        private void ShowWhereIsLastEvent()
        {
            if(LastEvent is BasicRequestEventViewModel requestEvent)
            {
                int floor = requestEvent.EventLocation.Id;
                _RequestViews[floor].Background = new SolidColorBrush(Colors.LightGreen);
            }

            if(LastEvent is ElevatorEventViewModel elevatorEvent)
            {
                int elevator = elevatorEvent.Elevator.Id;
                _ElevatorViews[elevator].Background = new SolidColorBrush(Colors.LightGreen);
            }
        }

        private void Redraw()
        {
            List<UIElement> itemsToRemove = new();

            foreach(UIElement element in surroundings.Children)
            {
                if (DrawnElementsUidsTracker.Contains(element.Uid))
                {
                    itemsToRemove.Add(element);
                }
            }

            foreach (UIElement element in building.Children)
            {
                if (DrawnElementsUidsTracker.Contains(element.Uid))
                {
                    itemsToRemove.Add(element);
                }
            }


            itemsToRemove.ForEach(element =>
            {
                surroundings.Children.Remove(element);
                building.Children.Remove(element);
                DrawnElementsUidsTracker.Remove(element.Uid);
            });

            Init();
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
                buildingView.Redraw();
            }
        }

        public void Update(object sender, RoutedEventArgs e)
        {
            UpdateViewAfterStep();
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
