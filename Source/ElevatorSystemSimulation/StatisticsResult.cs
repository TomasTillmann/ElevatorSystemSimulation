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

            AverageWaitingTimeOnFloor = CalcAverage(RequestInfos.Where(r => r is not null).Select(r => r.WaitingTimeOnFloor));
            AverageWaitingTimeInElevator = CalcAverage(RequestInfos.Where(r=> r is not null).Select(r => r.WaitingTimeInElevator));
            AverageElevatorIdleTime = CalcAverage(ElevatorInfos.Where(r=> r is not null).Select(e => e.TotalIdleTime));
            AverageServedRequestsPerElevatorCount = CalcAverage(ElevatorInfos.Where(r => r is not null).Select(e => e.ServedRequestsCount));

            MaxWaitingTimeOnFloor = RequestInfos.Where(r => r is not null).FindMaxSubset(r => r.WaitingTimeOnFloor.Value, int.MinValue).FirstOrDefault()?.WaitingTimeOnFloor ?? 0.ToSeconds();
            MaxWaitingTimeInElevator = RequestInfos.Where(r => r is not null).FindMaxSubset(r => r.WaitingTimeInElevator.Value, int.MinValue).FirstOrDefault()?.WaitingTimeInElevator ?? 0.ToSeconds();
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

            if(sum == 0 && count == 0)
            {
                return 0;
            }

            if(count == 0)
            {
                return int.MaxValue;
            }

            return sum / count; 
        }
    }
}
