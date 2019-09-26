using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // Marks a command as a command that can be looped multiple times.
    // i.e. randnum, coinflip, gimi
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CanLoopAttribute : Attribute
    {
        public CanLoopAttribute(){}
    }
}
