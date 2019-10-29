using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{

    // contains info relating to user configuration to orikivo
    public class UserOptions
    {
        private UserOptions() { }
        [JsonConstructor]
        internal UserOptions(string prefix, string nickname, Privacy? privacy = null)
        {
            Prefix = prefix;
            Nickname = nickname;
            Privacy = privacy ?? Default.Privacy;
        }

        public static Range NicknameLimit = new Range(2, 32, true);
        public static UserOptions Default
        {
            get
            {
                UserOptions userOptions = new UserOptions();
                userOptions.Prefix = null;
                userOptions.Privacy = Privacy.Public;
                // userOptions.ReplyOnEmojiName // this toggles if orikivo sends an emoji if you wrote the name of an emoji within your message.
                // userOptions.Privacy // this toggles what is visible to everyone within orikivo.
                userOptions.Nickname = null;
                return userOptions;
            }
        }

        /// <summary>
        /// The custom prefix that the user currently uses. Can be left empty.
        /// </summary>
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        private string _nickname;

        /// <summary>
        /// An optional nickname that overrides a global name.
        /// </summary>
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

        [JsonProperty("privacy")]
        public Privacy Privacy { get; internal set; }

        // TODO: Configure notifiers.
        public bool NotifyOnMeritEarn { get; internal set; }
    }
}
