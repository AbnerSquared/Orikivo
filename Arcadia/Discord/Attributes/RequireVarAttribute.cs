using System;

namespace Arcadia
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequireVarAttribute : Attribute
    {
        public RequireVarAttribute(string varId, VarOp match, long value)
        {
            VarId = varId;
            Operator = match;
            Value = value;
        }

        public string VarId { get; }

        public VarOp Operator { get; }

        public long Value { get; }
    }
}
