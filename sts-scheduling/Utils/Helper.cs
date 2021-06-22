using System;
using System.Collections.Generic;
using System.Linq;

namespace sts_scheduling.Utils
{
    public class Helper
    {
        /// <summary>
        /// C# equivalent of Python range (start, stop)
        /// </summary>
        /// <param name="start">The inclusive start.</param>
        /// <param name="stop">The exclusive stop.</param>
        /// <returns>A sequence of integers.</returns>
        public static IEnumerable<int> Range(int start, int stop)
        {
            foreach (var i in Enumerable.Range(start, stop - start))
                yield return i;
        }

        /// <summary>
        /// C# equivalent of Python range (stop)
        /// </summary>
        /// <param name="stop">The exclusive stop.</param>
        /// <returns>A sequence of integers.</returns>
        public static IEnumerable<int> Range(int stop)
        {
            return Range(0, stop);
        }
    }
}
