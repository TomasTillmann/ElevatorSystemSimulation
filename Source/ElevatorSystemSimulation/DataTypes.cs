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

        #endregion

        #region EqualsAndGetHashCode

        public override bool Equals(object? o)
        {
            if(o is Centimeters c)
            {
                return Value == c.Value;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #endregion
    }

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
                return Equals((Seconds)o);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #endregion
    }

    public enum Direction
    {
        Down = -1,
        NoDirection = 0,
        Up = 1,
    }
}
