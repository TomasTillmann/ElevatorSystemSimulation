namespace ElevatorSystemSimulation
{
    public struct Seconds
    {
        public int Value { get; }

        public Seconds(int value)
        {
            Value = value;
        }

        public override string ToString() => $"{Value} s";

        #region Operators

        public static Seconds operator +(Seconds s1, Seconds s2) => new(s1.Value + s2.Value);
        public static Seconds operator -(Seconds s1, Seconds s2) => new(s1.Value - s2.Value);
        public static Centimeters operator *(Seconds s, CentimetersPerSecond cps) => new(s.Value * cps.Value);
        public static bool operator <(Seconds s1, Seconds s2) => s1.Value < s2.Value;
        public static bool operator >(Seconds s1, Seconds s2) => s1.Value > s2.Value;
        public static bool operator ==(Seconds s1, Seconds s2) => s1.Value == s2.Value;
        public static bool operator !=(Seconds s1, Seconds s2) => s1.Value != s2.Value;

        #endregion

        #region EqualsAndGetHashCode

        public override bool Equals(object? o)
        {
            if (o is Seconds seconds)
            {
                return Value == seconds.Value; 
            }

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #endregion
    }
}
