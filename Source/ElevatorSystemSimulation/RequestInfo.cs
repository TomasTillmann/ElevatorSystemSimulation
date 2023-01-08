namespace ElevatorSystemSimulation
{
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
}
