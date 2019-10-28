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
                userOptions.DisplayFormat = EntityDisplayFormat.Text;
                return userOptions;
            }
        }

        /// <summary>
        /// The custom prefix that the user currently uses. Can be left empty.
        /// </summary>
        [Option]
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        private string _nickname;

        /// <summary>
        /// An optional nickname that overrides the name used on Orikivo.
        /// </summary>
        [Option]
        [JsonProperty("nickname")]
        public string Nickname
        {
            get => _nickname;
            set
            {
                if (Checks.NotNull(value))
                    if (NicknameLimit.ContainsValue(value.Length))
                        _nickname = value;
            }
        }

        
        /// <summary>
        /// Returns a value defining if the user set a custom prefix.
        /// </summary>
        [JsonIgnore]
        public bool HasPrefix => Checks.NotNull(Prefix);

        [Option]
        [JsonProperty("display_format")]
        public EntityDisplayFormat DisplayFormat { get; set; }

        [Option]
        [JsonProperty("privacy")]
        public Privacy Privacy { get; internal set; }

        // TODO: Configure notifiers.
        public bool NotifyOnMeritEarn { get; internal set; }
    }
}
