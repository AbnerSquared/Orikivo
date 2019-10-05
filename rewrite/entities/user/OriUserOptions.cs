using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // contains info relating to user configuration to orikivo
    public class OriUserOptions
    {
        private OriUserOptions() { }
        [JsonConstructor]
        internal OriUserOptions(string prefix, string nickname, EntityDisplayFormat? format = null, Privacy? privacy = null)
        {
            Prefix = prefix;
            Nickname = nickname;
            DisplayFormat = format ?? Default.DisplayFormat;
            Privacy = privacy ?? Default.Privacy;
        }

        public static Range NicknameLimit = new Range(2, 32, true);
        public static OriUserOptions Default
        {
            get
            {
                OriUserOptions userOptions = new OriUserOptions();
                userOptions.Prefix = null;
                userOptions.Privacy = Privacy.Public;
                // userOptions.ReplyOnEmojiName // this toggles if orikivo sends an emoji if you wrote the name of an emoji within your message.
                // userOptions.Privacy // this toggles what is visible to everyone within orikivo.
                userOptions.Nickname = null;
                userOptions.DisplayFormat = EntityDisplayFormat.Text; // remove this option? Poxel will be default, and text will be for non-Poxel.
                return userOptions;
            }
        }
        [Option]
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        private string _nickname;
        [Option]
        [JsonProperty("nickname")]
        public string Nickname
        {
            get { return _nickname; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    if (NicknameLimit.ContainsValue(value.Length))
                        _nickname = value;
            }
        }

        // static properties
        [JsonIgnore]
        public bool HasPrefix { get { return !string.IsNullOrWhiteSpace(Prefix); } }

        [Option]
        [JsonProperty("display_format")]
        public EntityDisplayFormat DisplayFormat { get; set; }

        [Option]
        [JsonProperty("privacy")]
        public Privacy Privacy { get; internal set; }
    }
}
