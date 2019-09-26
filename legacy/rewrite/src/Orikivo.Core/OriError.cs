using System.Collections.Generic;

namespace Orikivo
{
    public class OriError : Dictionary<string, string>
    {
        public OriError() : base()
        {
            Add("RangeNullException", "A range cannot have zero values.");
            Add("RangeOutOfAreaException", "The number that was specified was outside of the range.");
        }

        public new void Add(string type, string message)
            => base.Add(type, message);

        public new string this[string type]
        {
            get { return base[type]; }
            set { base[type] = value; } 
        }
    }
}