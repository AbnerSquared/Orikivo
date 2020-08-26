using System;

namespace Arcadia
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequireVarAttribute : Attribute
    {
        public RequireVarAttribute(string varId, VarMatch match, long value)
        {
            VarId = varId;
            Matcher = match;
            Value = value;
        }

        public string VarId { get; }
        public VarMatch Matcher { get; }
        public long Value { get; }
    }
}