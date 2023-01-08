using ElevatorSystemSimulation.Extensions;

namespace ElevatorSystemSimulation
{
    public class StatisticsResult
    {
        public List<RequestInfo> RequestInfos { get; }
        public List<ElevatorInfo> ElevatorInfos { get; }

        public Seconds AverageWaitingTimeOnFloor { get; }
        public Seconds MaxWaitingTimeOnFloor { get; }   
        public Seconds AverageWaitingTimeInElevator { get; }
        public Seconds MaxWaitingTimeInElevator { get; }
        public Seconds AverageElevatorIdleTime { get; }
        public int AverageServedRequestsPerElevatorCount { get; }

        public StatisticsResult(List<RequestInfo> requestInfos, List<ElevatorInfo> elevatorInfos)
        {
            RequestInfos = requestInfos;
            ElevatorInfos = elevatorInfos;

            AverageWaitingTimeOnFloor = CalcAverage(RequestInfos.Select(r => r.WaitingTimeOnFloor));
            AverageWaitingTimeInElevator = CalcAverage(RequestInfos.Select(r => r.WaitingTimeInElevator));
            AverageElevatorIdleTime = CalcAverage(ElevatorInfos.Select(e => e.TotalIdleTime));
            AverageServedRequestsPerElevatorCount = CalcAverage(ElevatorInfos.Select(e => e.ServedRequestsCount));

            MaxWaitingTimeOnFloor = RequestInfos.FindMaxSubset(r => r.WaitingTimeOnFloor.Value, int.MinValue).First().WaitingTimeOnFloor;
            MaxWaitingTimeInElevator = RequestInfos.FindMaxSubset(r => r.WaitingTimeInElevator.Value, int.MinValue).First().WaitingTimeInElevator;
        }

        private static Seconds CalcAverage(IEnumerable<Seconds> values)
        {
            return CalcAverage(values.Select(v => v.Value)).ToSeconds();
        }

        private static int CalcAverage(IEnumerable<int> values)
        {
            int sum = 0;
            int count = 0;

            foreach (int value in values)
            {
                sum += value;
                count += 1;
            }

            return sum / count; 
        }
    }
}
