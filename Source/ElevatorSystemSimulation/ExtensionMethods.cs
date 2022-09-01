using DataTypes;

namespace Extensions {
    public static class ExtensionMethods {
        public static Seconds ToSeconds(this int value) => new(value);
    }
}
