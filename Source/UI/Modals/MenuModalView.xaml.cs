using Client;
using ElevatorSystemSimulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class MenuModalView : Window
    {
        public MenuModalViewModel ViewModel => (MenuModalViewModel)DataContext;

        public DataTemplate ElevatorSystemPickerMainContentTemplate { get { return (DataTemplate)GetValue(ElevatorSystemPickerMainContentTemplateProperty); } set { SetValue(ElevatorSystemPickerMainContentTemplateProperty, value); } }
        public static readonly DependencyProperty ElevatorSystemPickerMainContentTemplateProperty = DependencyProperty.Register("ElevatorSystemPickerMainContentTemplate", typeof(DataTemplate), typeof(MenuModalView));

        public Simulation<BasicRequest>? ResultingSimulation { get { return (Simulation<BasicRequest>?)GetValue(ResultingSimulationProperty); } set { SetValue(ResultingSimulationProperty, value); } }
        public static readonly DependencyProperty ResultingSimulationProperty = DependencyProperty.Register("ResultingSimulation", typeof(Simulation<BasicRequest>), typeof(MenuModalView));

        public MenuModalView(Window owner, Building building)
        {
            Owner = owner;
            Owner.Opacity = 0.5;

            InitializeComponent();

            ViewModel.Owner = owner;
            building.ElevatorSystem.Elevators.Select(e => new ElevatorInModalViewModel(e)).ToList().ForEach(e => ViewModel.Elevators.Add(e));
            building.Floors.Value.Select(f => new FloorInModalViewModel(f)).ToList().ForEach(f => ViewModel.Floors.Add(f));
            ViewModel.RequestsTimeSpan = building.Population.RequestsTimeSpan.Value;
            ViewModel.RequestsCount = building.Population.RequestsCount;
            ViewModel.Seed = building.Population.Seed;

            // update resulting simulation when updated in view model - can be than extracted by owner window
            SetBinding(ResultingSimulationProperty, new Binding("ResultingSimulation") { Source = ViewModel });

            viewComboBox.ItemsSource = Enum.GetValues(typeof(SelectionStatesInModalView)).Cast<SelectionStatesInModalView>();
        }
    }

    public enum SelectionStatesInModalView
    {
        Elevators,
        Floors,
        Requests,
        Logic,
    }
}
