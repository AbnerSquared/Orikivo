using System;

namespace Orikivo
{
    public sealed class ServiceAttribute : Attribute
    {
        public ServiceAttribute(ServiceType type = ServiceType.Singleton)
        {
            Type = type;
        }

        public ServiceType Type { get; private set; }
    }
}