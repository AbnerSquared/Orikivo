namespace Arcadia
{
    public class StackRange
    {
        public StackRange(int min = 1, int? max = null)
        {
            Min = min;
            Max = max ?? min;
        }

        /// <summary>
        /// The minimum amount to return.
        /// </summary>
        public int Min { get; set; } = 1;

        /// <summary>
        /// The maximum amount to return.
        /// </summary>
        public int Max { get; set; } = 1;

        public static implicit operator StackRange(int value)
        {
            return new StackRange(value);
        }
    }

    public readonly struct ComponentRange
    {
        public static readonly ComponentRange Default = new ComponentRange();

        public ComponentRange(int min = int.MinValue, int max = int.MaxValue)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; }
        public int Max { get; }

        public static implicit operator ComponentRange(int value)
        {
            return new ComponentRange(value, value);
        }
    }
}
