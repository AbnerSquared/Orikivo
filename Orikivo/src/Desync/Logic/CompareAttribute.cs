using System;

namespace Orikivo.Desync
{

    // this checks flags and stats; flags are simply a 0 OR 1 state, and stats have a long value equipped. The Engine class handles the object type that stat actually stores
    // stats might be converted to bytes.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class OnlyWhenAttribute : Attribute
    {
        public OnlyWhenAttribute(LogicGate gate, string flag, params string[] rest) { }
        public OnlyWhenAttribute(string valueA, LogicMatch match, long valueB) { }
        public OnlyWhenAttribute(LogicGate gate, (string, LogicMatch, long) testA, (string, LogicMatch, long) testB, params (string, LogicMatch, long)[] rest) { }
    
        private LogicGate Gate { get; }
        private (string, LogicMatch, long)[] Matches { get; }
        private string[] Flags { get; }
    }
}