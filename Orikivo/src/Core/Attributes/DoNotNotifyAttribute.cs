using System;

namespace Orikivo
{
    // If this attribute is on a method, do not display any notifications to the user.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DoNotNotifyAttribute : Attribute
    {

    }
}