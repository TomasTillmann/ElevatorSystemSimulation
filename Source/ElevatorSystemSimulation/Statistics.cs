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
    public class Statistics<TRequest> where TRequest : Request
    {
        /// <summary>
        /// When this is triggered by simulation, it means, that the request just happened.
        /// </summary>
        /// <param name="state"></param>
        public void Update(ISimulationState<TRequest> state)
        {
            // start of waiting in floor
            state.Event.Info = new RequestInfo(0.ToSeconds(), 0.ToSeconds());
            state.Event.Info.StartOfWaiting = state.Time;
        }

        public void Update(ISimulationState<ElevatorEvent> state)
        {
            Elevator elevator = state.Event.Elevator;
            Floor currentFloor = state.Event.EventLocation;

            switch (state.Event.FinishedAction)
            {
                case ElevatorAction.UnloadAndLoad:
                    UpdateAfterUnload(state);
                    UpdateAfterLoad(state);

                    break;

                case ElevatorAction.Load:
                    UpdateAfterLoad(state);

                    break;

                case ElevatorAction.Unload:
                    UpdateAfterUnload(state);

                    break;

                case ElevatorAction.MoveTo:
                    break;

                case ElevatorAction.Idle:
                    elevator.Info.StartOfIdle = state.Time;
                    break;
            }

            if(state.Event.FinishedAction != ElevatorAction.Idle)
            {
                elevator.Info.TotalIdleTime += elevator.Info.StartOfIdle + (state.Time - elevator.Info.StartOfIdle);
            }
        }

        public StatisticsResult GetResult(List<Request> requests, List<Elevator> elevators)
        {
            return new StatisticsResult
            (
                requests.Select(r => r.Info).ToList(),
                elevators.Select(e => e.Info).ToList()
            );
        }

        private void UpdateAfterLoad(ISimulationState<ElevatorEvent> state)
        {
            Elevator elevator = state.Event.Elevator;
            elevator.Info.DeparturesCount += 1;

            IEnumerable<Request> freshlyInElevator = elevator.AttendingRequests.Where(r => r.Info.WaitingTimeOnFloor == 0.ToSeconds());
            foreach (Request req in freshlyInElevator)
            {
                // end of waiting in floor
                req.Info.WaitingTimeOnFloor = req.Info.StartOfWaiting + (state.Time - req.Info.StartOfWaiting);
                req.Info.ServingElevator = elevator;

                // now start of waiting in elevator
                req.Info.StartOfWaiting = state.Time;
            }
        }

        private void UpdateAfterUnload(ISimulationState<ElevatorEvent> state)
        {
            Elevator elevator = state.Event.Elevator;
            elevator.Info.DeparturesCount += 1;

            // departing can happen only after finished unload action 
            foreach(Request req in state.Event.DepartedRequests)
            {
                // end of waiting in elevator
                req.Info.WaitingTimeInElevator = req.Info.StartOfWaiting + (state.Time - req.Info.StartOfWaiting);
                elevator.Info.ServedRequestsCount += 1;
            }
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

    public class RequestInfo
    {
        public Elevator? ServingElevator { get; set; }
        public Seconds WaitingTimeOnFloor { get; set; }
        public Seconds WaitingTimeInElevator { get; set; }

        #region InternalStatsHandling

        internal Seconds StartOfWaiting { get; set; }

        #endregion

        public RequestInfo(Seconds waitingTimeOnFloor, Seconds waitingTimeInElevator, Elevator? servingElevator = null)
        {
            WaitingTimeOnFloor = waitingTimeOnFloor;
            WaitingTimeInElevator = waitingTimeInElevator;
            ServingElevator = servingElevator;
        }
    }

    public class ElevatorInfo
    {
        public Seconds TotalIdleTime { get; set; }
        public int DeparturesCount { get; set; }
        public int ServedRequestsCount { get; set; }

        #region InternalStatsHandling

        internal Seconds StartOfIdle { get; set; }

        #endregion

        public ElevatorInfo(Seconds totalIdleTime, int departuresCount, int servedRequestsCount)
        {
            TotalIdleTime = totalIdleTime;
            DeparturesCount = departuresCount;
            ServedRequestsCount = servedRequestsCount;
        }
    }
}
