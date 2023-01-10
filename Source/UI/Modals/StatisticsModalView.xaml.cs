using Client;
using ElevatorSystemSimulation;
using ElevatorSystemSimulation.Interfaces;
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

        public ExportInfo ExportInfo { get { return (ExportInfo)GetValue(ExportInfoProperty); } set { SetValue(ExportInfoProperty, value); } }
        public static readonly DependencyProperty ExportInfoProperty = DependencyProperty.Register("ExportInfoProperty", typeof(ExportInfo), typeof(StatisticsModalView));

        public StatisticsModalView(Window owner, StatisticsResult stats, ISimulation<BasicRequest> simulation)
        {
            Owner = owner;
            Owner.Opacity = 0.5;
            Stats = stats;
            string exportFileName = $"{simulation.CurrentLogic}_{simulation.Building.ElevatorSystem.Elevators.Count}_{simulation.Building.Floors.Value.Count}_{DateTime.UtcNow.Date.DayOfYear}.{DateTime.UtcNow.Date.Month}_{DateTime.UtcNow.Hour}h{DateTime.UtcNow.Minute}m{DateTime.UtcNow.Second}s";
            ExportInfo = new(exportFileName, new StatsViewModel(stats));

            InitializeComponent();
        }
    }

    public class ExportInfo
    {
        public StatsViewModel Stats { get; }
        public string ExportFileName { get; }

        public ExportInfo(string exportFileName, StatsViewModel stats)
        {
            ExportFileName = exportFileName;
            Stats = stats;
        }
    }

    public class StatsViewModel : ViewModelBase<StatisticsResult>
    {
        public StatsViewModel(StatisticsResult model) : base(model) { }

        //TODO: use string builder
        public string Serialize()
        {
            string res = "";
            res += "Statistics:" + "\n";
            res += "Average waiting time on floor: " + Model.AverageWaitingTimeOnFloor + "\n";
            res += "Average waiting time in elevator: " + Model.AverageWaitingTimeInElevator + "\n";
            res += "Average elevator idle time: " + Model.AverageElevatorIdleTime + "\n";
            res += "Average served requests per elevator count: " + Model.AverageServedRequestsPerElevatorCount + "\n";
            res += "Max waiting time in elevator: " + Model.MaxWaitingTimeInElevator + "\n";
            res += "Max waiting time on floor: " + Model.MaxWaitingTimeOnFloor + "\n";

            res += "\n";
            res += "Elevator infos:" + '\n';
            foreach(ElevatorInfo eInfo in Model.ElevatorInfos)
            {
                res += $"Elevator: {eInfo.ElevatorId}" + "\n";
                res += "Total Idle time: " + eInfo.TotalIdleTime + "\n";
                res += "Total departures count: " + eInfo.DeparturesCount + "\n";
                res += "Served requests count: " + eInfo.ServedRequestsCount + "\n";
                res += "\n";
            }
            res += "\n";

            res += "Request infos:" + "\n";
            foreach(RequestInfo rInfo in Model.RequestInfos.Where(r => r is not null))
            {
                res += "Assigned elevator: " + $"{rInfo.ServingElevator?.Id.ToString() ?? "No assigned yet"}" + "\n";
                res += "Waiting time in elevator: " + $"{rInfo.WaitingTimeInElevator}" + "\n";
                res += "Waiting time on floor: " + $"{rInfo.WaitingTimeOnFloor}" + "\n";
                res += "\n";
            }
            res += "\n";

            return res;
        }
    }
}
