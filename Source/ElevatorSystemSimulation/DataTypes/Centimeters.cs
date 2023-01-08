namespace ElevatorSystemSimulation
{
    public struct Centimeters
    {
        public int Value { get; }
        public Centimeters(int value)
        {
            Value = value;
        }

        public override string ToString() => $"{Value} cm";

        #region Operators
        public static Centimeters operator +(Centimeters m1, Centimeters m2) => new(m1.Value + m2.Value);
        public static Centimeters operator -(Centimeters m1, Centimeters m2) => new(m1.Value - m2.Value);
        public static Seconds operator /(Centimeters distance, CentimetersPerSecond velocity) => new(distance.Value / velocity.Value);
        public static bool operator == (Centimeters c1, Centimeters c2) => c1.Value == c2.Value;
        public static bool operator !=(Centimeters c1, Centimeters c2) => c1.Value != c2.Value;
        public static Centimeters operator *(Direction direction, Centimeters c) => new((int)direction * c.Value);
        public static bool operator <(Centimeters c1, Centimeters c2) => c1.Value < c2.Value;
        public static bool operator <=(Centimeters c1, Centimeters c2) => c1.Value <= c2.Value;
        public static bool operator >=(Centimeters c1, Centimeters c2) => c1.Value >= c2.Value;
        public static bool operator >(Centimeters c1, Centimeters c2) => c1.Value > c2.Value;

        #endregion

        #region EqualsAndGetHashCode

        public override bool Equals(object? o)
        {
            if(o is Centimeters centimeters)
            {
                return Value == centimeters.Value;
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
