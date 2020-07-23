namespace Orikivo.Desync
{
    public class ExpReward
    {
        public ExpReward(ExpType type, ulong value)
        {
            Type = type;
            Value = value;
        }

        public ExpType Type { get; set; }
        public ulong Value { get; set; }
    }
}
