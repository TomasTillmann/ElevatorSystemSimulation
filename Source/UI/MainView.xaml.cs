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
        public double SidePanelScale { get { return (double)GetValue(SidePanelScaleProperty); } set { SetValue(SidePanelScaleProperty, value); } }
        public static readonly DependencyProperty SidePanelScaleProperty = DependencyProperty.Register("SidePanelScale", typeof(double), typeof(BuildingView), new PropertyMetadata(1.0));

        public MainView()
        {
            InitializeComponent();

            stepButton.Click += DataContext.Step;
            stepButton.Click += buildingView.Update;

            stepBackButton.Click += DataContext.StepBack;
            stepBackButton.Click += buildingView.Update;

            sidePanel.PreviewMouseWheel += OnPreviewMouseWheelMoving;

            restartButton.Click += DataContext.Restart;
            restartButton.Click += buildingView.Update;
        }

        #region SidePanelScaling
        private void OnPreviewMouseWheelMoving(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta < 0)
            {
                if (SidePanelScale >= 0.2)
                {
                    double scale = SidePanelScale - 0.1;
                    if (scale >= 0.01)
                    {
                        SidePanelScale = scale;
                    }
                }
                else
                {
                    double scale = SidePanelScale - 0.05;
                    if (scale >= 0.01)
                    {
                        SidePanelScale = scale;
                    }
                }

            }
            else if (e.Delta > 0)
            {
                SidePanelScale += 0.1;
            }
        }
        #endregion
    }
}
