namespace Orikivo.Text
{
    internal class NodeValue
    {
        internal NodeValue(int index, string value)
        {
            Index = index;
            Value = value;
        }

        public int Index { get; }
        public string Value { get; }
    }
}
