namespace Arcadia
{
    public class RuleAction
    {
        public string RuleId { get; set; }

        // the action ID called if true; can be left null
        public string OnTrue { get; set; }

        // the action ID called if false; can be left null
        public string OnFalse { get; set; }

        // if invalid, throw an exception as this rule is now stuck
        public bool IsValid()
        {
            return string.IsNullOrWhiteSpace(OnTrue) && string.IsNullOrWhiteSpace(OnFalse);
        }
    }
}
