using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class IdAttribute : Attribute
    {
        public IdAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
