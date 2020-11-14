using System;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class IdAttribute : Attribute
    {
        public IdAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
