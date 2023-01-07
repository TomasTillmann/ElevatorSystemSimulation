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
    public class Statistics
    {
        private Dictionary<int, RequestInfo> requestInfos = new();
        private Dictionary<int, ElevatorInfo> elevatorInfos = new();

        internal void UpdateRequestsOnFloorStats(Floors floors, Seconds duration, Seconds currentTime)
        {
            // requests not dealt by elevators yet 
            IEnumerable<IRequestEvent> activeRequests = floors.GetAllActiveRequests();

            foreach(IRequestEvent request in activeRequests)
            {
                if (requestInfos.TryGetValue(request.Id, out RequestInfo? requestInfo))
                {
                    requestInfos[request.Id].UpdateStats(duration);
                }
                else
                {
                    requestInfos.Add(request.Id, new RequestInfo(request, null, currentTime - request.WhenPlanned, 0.ToSeconds()));
                }
            }
        }

        internal void UpdateRequestsInElevatorsStats(ElevatorSystem elevatorSystem, Seconds duration, Seconds currentTime)
        {
            foreach(Elevator elevator in elevatorSystem.Value)
            {
                foreach(IRequestEvent request in elevator.AttendingRequests)
                {
                    // if is in elevator, it had to be added from the floor
                    requestInfos[request.Id].ServingElevator = elevator;
                    requestInfos[request.Id].WaitingTimeInElevator += duration;
                }
            }
        }

        public void Measure()
        {

        }

        public StatisticsResult GetResult()
        {
            throw new NotImplementedException();
        }
    }

    public struct StatisticsResult
    {

    }

    internal class RequestInfo : IIdentifiable
    {
        public IRequestEvent Request { get; set; }
        public Elevator? ServingElevator { get; set; }
        public Seconds WaitingTimeOnFloor { get; set; }
        public Seconds WaitingTimeInElevator { get; set; }

        public int Id => Request.Id;

        public void UpdateStats(Seconds duration)
        {
            WaitingTimeOnFloor += duration; 

            if(ServingElevator is not null)
            {
                WaitingTimeInElevator += duration;
            }
        }

        public RequestInfo(IRequestEvent request, Elevator? servingElevator, Seconds waitingTimeOnFloor, Seconds waitingTimeInElevator)
        {
            Request = request;
            ServingElevator = servingElevator;
            WaitingTimeOnFloor = waitingTimeOnFloor;
            WaitingTimeInElevator = waitingTimeInElevator;
        }
    }

    internal class ElevatorInfo : IIdentifiable
    {
        public Elevator Elevator { get; set; }
        public Seconds TotalInUseTime { get; set; }
        public Seconds TotalIdleTime { get; set; }
        public int DeparturesCount { get; set; }
        public int ServedRequestsCount { get; set; }

        public int Id => Elevator.Id;

        public ElevatorInfo(Elevator elevator, Seconds totalInUseTime, Seconds totalIdleTime, int departuresCount, int servedRequestsCount)
        {
            Elevator = elevator;
            TotalInUseTime = totalInUseTime;
            TotalIdleTime = totalIdleTime;
            DeparturesCount = departuresCount;
            ServedRequestsCount = servedRequestsCount;
        }
    }
}
