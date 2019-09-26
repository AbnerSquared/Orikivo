using Newtonsoft.Json;
using System;

namespace Orikivo
{
    public class UserRoleInfo
    {
        public UserRoleInfo(ulong userId, ulong roleId, double? seconds = null)
        {
            UserId = userId;
            RoleId = roleId;
            if (seconds.HasValue)
                ExpiresOn = DateTime.UtcNow.AddSeconds(seconds.Value);
        }

        [JsonProperty("user_id")]
        public ulong UserId { get; }
        [JsonProperty("role_id")]
        public ulong RoleId { get; }

        [JsonProperty("expires_on")]
        public DateTime? ExpiresOn { get; set; }

        [JsonIgnore]
        public bool HasExpired => ExpiresOn.HasValue ? (DateTime.UtcNow - ExpiresOn.Value).TotalSeconds <= 0 : false;
    }
}
