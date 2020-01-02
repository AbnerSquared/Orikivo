using System;

namespace Orikivo.Unstable
{
    public class CooldownData
    {
        public CooldownData(string id, DateTime expiresOn)
        {
            Id = id;
            ExpiresOn = expiresOn;
        }

        public string Id { get; }
        public DateTime ExpiresOn { get; }
    }
}
