using ElevatorSystemSimulation.Extensions;
using ElevatorSystemSimulation.Interfaces;

namespace ElevatorSystemSimulation
{
    // Measures:
    // Average and max and min waiting times on the floor and in the elevator
    // Max number of people at one floor
    // Total distance traveled by each elevator
    // Total number of departures by each elevator
    // Total Idle time and in use time by each elevator
    public class Statistics<TRequestEvent> where TRequestEvent : RequestEvent
    {
        private Dictionary<int, RequestInfo> requestInfos = new();
        private Dictionary<int, ElevatorInfo> elevatorInfos = new();

        public void Update(ISimulationState<TRequestEvent> state)
        {

        }

        public void Update(ISimulationState<ElevatorEvent> state)
        {

        }

        public StatisticsResult GetResult()
        {
            return new StatisticsResult(requestInfos.Values.ToList(), elevatorInfos.Values.ToList());
        }
    }

    public class StatisticsResult
    {
        public List<RequestInfo> RequestInfos { get; }
        public List<ElevatorInfo> ElevatorInfos { get; }

        public Seconds AverageWaitingTimeOnFloor { get; }
        public Seconds MaxWaitingTimeOnFloor { get; }
        public Seconds AverageWaitingTimeInElevator { get; }
        public Seconds MaxWaitingTimeInElevator { get; }
        public int ServedRequests { get; }
        public Seconds TotalTime { get; }
        public Seconds AverageElevatorIdleTime { get; }
        public int AverageRequestsPerElevatorCount { get; }

        public StatisticsResult(List<RequestInfo> requestInfos, List<ElevatorInfo> elevatorInfos)
        {
            RequestInfos = requestInfos;
            ElevatorInfos = elevatorInfos;

            // TODO: will be all calculated from these two above
        }
    }

    public class RequestInfo
    {
        public Elevator? ServingElevator { get; set; }
        public Seconds WaitingTimeOnFloor { get; set; }
        public Seconds WaitingTimeInElevator { get; set; }

        public RequestInfo(Elevator? servingElevator, Seconds waitingTimeOnFloor, Seconds waitingTimeInElevator)
        {
            ServingElevator = servingElevator;
            WaitingTimeOnFloor = waitingTimeOnFloor;
            WaitingTimeInElevator = waitingTimeInElevator;
        }
    }

    public class ElevatorInfo
    {
        public Seconds TotalInUseTime { get; set; }
        public Seconds TotalIdleTime { get; set; }
        public int DeparturesCount { get; set; }
        public int ServedRequestsCount { get; set; }

        public ElevatorInfo(Seconds totalInUseTime, Seconds totalIdleTime, int departuresCount, int servedRequestsCount)
        {
            TotalInUseTime = totalInUseTime;
            TotalIdleTime = totalIdleTime;
            DeparturesCount = departuresCount;
            ServedRequestsCount = servedRequestsCount;
        }
    }
}
