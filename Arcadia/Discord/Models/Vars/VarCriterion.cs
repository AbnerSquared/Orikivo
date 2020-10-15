using System;

namespace Arcadia
{
    public class VarCriterion : Criterion
    {
        public VarCriterion(string id, long expectedValue, bool judgeAsNew = true)
        {
            Id = id;
            ExpectedValue = expectedValue;
            Judge = ctx => ctx.User.GetVar(id) >= ExpectedValue;
            JudgeAsNew = judgeAsNew;
        }

        public long ExpectedValue { get; }

        // This determines if the criterion that is specified is tracked on a blank slate or not
        public bool JudgeAsNew { get; }
    }
}
