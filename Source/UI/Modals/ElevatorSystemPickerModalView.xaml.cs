using Client;
using ElevatorSystemSimulation;
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

        public Simulation<BasicRequest>? ResultingSimulation { get { return (Simulation<BasicRequest>?)GetValue(ResultingSimulationProperty); } set { SetValue(ResultingSimulationProperty, value); } }
        public static readonly DependencyProperty ResultingSimulationProperty = DependencyProperty.Register("ResultingSimulation", typeof(Simulation<BasicRequest>), typeof(ElevatorSystemPickerModalView));

        public ElevatorSystemPickerModalView(Window owner)
        {
            Owner = owner;
            Owner.Opacity = 0.5;
            InitializeComponent();

            // update resulting simulation when updated in view model - can be than extracted by owner window
            SetBinding(ResultingSimulationProperty, new Binding("ResultingSimulation") { Source = ViewModel });

            viewComboBox.ItemsSource = Enum.GetValues(typeof(SelectionStatesInModalView)).Cast<SelectionStatesInModalView>();
        }
    }

    public enum SelectionStatesInModalView
    {
        Elevators,
        Floors,
        Logic,
    }
}
