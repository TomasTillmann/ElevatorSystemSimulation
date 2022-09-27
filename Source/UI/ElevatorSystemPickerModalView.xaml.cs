using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class ElevatorSystemPickerModalView : Window
    {
        public ElevatorSystemPickerViewModel ViewModel => (ElevatorSystemPickerViewModel)DataContext;

        public DataTemplate ElevatorSystemPickerMainContentTemplate { get { return (DataTemplate)GetValue(ElevatorSystemPickerMainContentTemplateProperty); } set { SetValue(ElevatorSystemPickerMainContentTemplateProperty, value); } }
        public static readonly DependencyProperty ElevatorSystemPickerMainContentTemplateProperty = DependencyProperty.Register("ElevatorSystemPickerMainContentTemplate", typeof(DataTemplate), typeof(ElevatorSystemPickerModalView));

        public ElevatorSystemPickerModalView(Window owner)
        {
            Owner = owner;
            Owner.Opacity = 0.5;
            InitializeComponent();

            Width = owner.Width - 2 * owner.Width / 3;
            Height = owner.Height - 2 * owner.Height / 10;

            Closing += OnWindowClosing;
            cancelButton.Click += CloseWindow;

            viewComboBox.ItemsSource = Enum.GetValues(typeof(ViewType)).Cast<ViewType>();
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            Owner.Opacity = 1;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnElevatorsPicker(object sender, RoutedEventArgs e)
        {
            ElevatorSystemPickerMainContentTemplate = (DataTemplate)TryFindResource("elevatorPickerDataTemplate");
        }

        private void OnFloorsPicker(object sender, RoutedEventArgs e)
        {
            ElevatorSystemPickerMainContentTemplate = (DataTemplate)TryFindResource("floorsPickerDataTemplate");
        }
        private void OnAlgorithmPicker(object sender, RoutedEventArgs e)
        {
            ElevatorSystemPickerMainContentTemplate = (DataTemplate)TryFindResource("algorithmPickerDataTemplate");
        }

        public enum ViewType
        {
            Elevators,
            Floors,
            Algorithm,
        }
    }
}
