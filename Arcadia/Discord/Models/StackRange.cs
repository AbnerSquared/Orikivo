namespace Arcadia
{
    public class StackRange
    {
        public StackRange(int min = 1, int max = 1)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; set; } = 1;
        public int Max { get; set; } = 1;

        public static implicit operator StackRange(int value)
        {
            return new StackRange(value, value);
        }
    }
}
