using System.Collections.Generic;

namespace Orikivo.Utility
{
    public class OriKeyBatch
    {
        public OriKeyBatch(string value, int iterations, int originalIndex, List<string> keys)
        {
            Value = value;
            IterationCount = iterations;
            OriginalIndex = originalIndex;
            Keys = keys;
        }

        public List<string> Keys { get; }
        public string Value { get; }
        public int IterationCount { get; }
        public int OriginalIndex { get; }
    }
}
