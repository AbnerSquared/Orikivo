using System;

namespace Orikivo
{
    public class OriMessageInvokerConfig
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
