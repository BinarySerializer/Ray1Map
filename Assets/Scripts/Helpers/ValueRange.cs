using System.Collections.Generic;
using System.Linq;

namespace Ray1Map
{
    public class ValueRange
    {
        public ValueRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; }
        public int Max { get; }

        public IEnumerable<int> EnumerateRange() => Enumerable.Range(Min, Max - Min + 1);

        public static IEnumerable<int> EnumerateRanges(params ValueRange[] ranges) => ranges.SelectMany(x => x.EnumerateRange());
    }
}