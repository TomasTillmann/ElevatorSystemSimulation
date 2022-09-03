using DataTypes;

namespace Extensions
{
    public static class ExtensionMethods
    {
        public static Seconds ToSeconds (this int value) => new(value);
        public static CentimetersPerSecond ToCmPerSec (this int value) => new(value);
        public static Centimeters ToCentimeters (this int value) => new(value);
    }
}
