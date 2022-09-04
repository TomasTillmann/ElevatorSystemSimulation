namespace ElevatorSystemSimulation
{
    public struct CentimetersPerSecond
    {
        public int Value { get; }

        public CentimetersPerSecond(int value)
        {
            Value = value;
        }

        #region Operators
        public static Centimeters operator *(CentimetersPerSecond speed, TimeSpan time) => new(speed.Value * time.Seconds);
        public static Centimeters operator *(TimeSpan time, CentimetersPerSecond speed) => new(speed.Value * time.Seconds);
        #endregion
    }

    public struct Centimeters
    {
        public int Value { get; }
        public Centimeters(int value)
        {
            Value = value;
        }

        #region Operators
        public static Centimeters operator +(Centimeters m1, Centimeters m2) => new(m1.Value + m2.Value);
        public static Centimeters operator -(Centimeters m1, Centimeters m2) => new(m1.Value - m2.Value);
        public static Seconds operator /(Centimeters distance, CentimetersPerSecond velocity) => new(distance.Value / velocity.Value);
        public static bool operator == (Centimeters c1, Centimeters c2) => c1.Value == c2.Value;
        public static bool operator !=(Centimeters c1, Centimeters c2) => c1.Value != c2.Value;

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

        #region Operators

        public static Seconds operator +(Seconds s1, Seconds s2) => new(s1.Value + s2.Value);
        public static Seconds operator -(Seconds s1, Seconds s2) => new(s1.Value - s2.Value);
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

    public class PopulationDistribution
    {
        public DateTime FromDateTime { get; set; }
        public DateTime ToDateTime { get; set; }

        // TODO add actual distribution representation
    }

    public enum Direction
    {
        Down = -1,
        Up = 1,
    }
}
