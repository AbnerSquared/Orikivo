using System;

namespace Arcadia
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequireVarAttribute : Attribute
    {
        public RequireVarAttribute(string varId, VarMatch match, long value)
        {
            VarId = varId;
            Match = match;
            Value = value;
        }

        public string VarId { get; }
        public VarMatch Match { get; }
        public long Value { get; }
    }
}