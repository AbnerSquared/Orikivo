using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : Attribute { }
}
