namespace Orikivo
{
    // an on/off state for a string
    public class BooleanText
    {
        public BooleanText(string onTrue, string onFalse)
        {
            OnTrue = onTrue;
            OnFalse = onFalse;
        }

        public string OnTrue { get; }
        public string OnFalse { get; }

        public string Get(bool state)
            => state ? OnTrue : OnFalse;
    }
}
