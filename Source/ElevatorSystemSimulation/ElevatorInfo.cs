namespace ElevatorSystemSimulation
{
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
