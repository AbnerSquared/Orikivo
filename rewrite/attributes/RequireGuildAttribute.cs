using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// An attribute that marks a command to require its execution channel to be within a guild.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireGuildAttribute : Attribute {}
}
