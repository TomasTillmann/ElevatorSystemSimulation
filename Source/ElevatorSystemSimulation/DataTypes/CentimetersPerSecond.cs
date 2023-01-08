namespace ElevatorSystemSimulation
{
    public struct CentimetersPerSecond
    {
        public int Value { get; }

        public CentimetersPerSecond(int value)
        {
            Value = value;
        }

        public override string ToString() => $"{Value} cm/s";

        #region Operators
        public static Centimeters operator *(CentimetersPerSecond speed, Seconds time) => new(speed.Value * time.Value);
        public static Centimeters operator *(Seconds time, CentimetersPerSecond speed) => new(speed.Value * time.Value);
        #endregion
    }
}
