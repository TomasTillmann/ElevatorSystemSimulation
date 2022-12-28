namespace ElevatorSystemSimulation
{
    namespace Extensions
    {
        public static class ExtensionMethods
        {
            public static Seconds ToSeconds (this int value) => new(value);
            public static CentimetersPerSecond ToCmPerSec (this int value) => new(value);
            public static Centimeters ToCentimeters (this int value) => new(value);

            /// <summary>
            /// returns subset for with minimal value, with at least one element
            /// </summary>
            /// <typeparam name="MinValueType"></typeparam>
            /// <typeparam name="BaseType"></typeparam>
            /// <param name="stuff"></param>
            /// <param name="dataTransform"></param>
            /// <param name="infinity"></param>
            /// <returns></returns>
            public static List<BaseType> FindMinSubset<MinValueType, BaseType>(this IEnumerable<BaseType> stuff, Func<BaseType, MinValueType> dataTransform, MinValueType infinity) where MinValueType : IComparable<MinValueType>
            {
                MinValueType minValue = infinity;
                List<BaseType> result = new();

                foreach (BaseType s in stuff)
                {
                    MinValueType value = dataTransform(s);
                    // value < minValue
                    if (value.CompareTo(minValue) == -1)
                    {
                        minValue = value;
                        result.Clear();
                        result.Add(s);
                    }
                    // value == minValue
                    else if (value.CompareTo(minValue) == 0)
                    {
                        result.Add(s);
                    }
                }

                return result;
            }

            public static List<BaseType> FindMaxSubset<MaxValueType, BaseType>(this IEnumerable<BaseType> stuff, Func<BaseType, MaxValueType> dataTransform, MaxValueType negativeInfinity) where MaxValueType : IComparable<MaxValueType>
            {
                MaxValueType maxValue = negativeInfinity;
                List<BaseType> result = new();

                foreach (BaseType s in stuff)
                {
                    MaxValueType value = dataTransform(s);
                    // value > maxValue
                    if (value.CompareTo(maxValue) == 1)
                    {
                        maxValue = value;
                        result.Clear();
                        result.Add(s);
                    }
                    // value == maxValue
                    else if (value.CompareTo(maxValue) == 0)
                    {
                        result.Add(s);
                    }
                }

                return result;
            }
        }
    }
}
