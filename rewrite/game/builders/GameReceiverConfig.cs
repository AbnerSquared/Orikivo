using Discord;

namespace Orikivo
{
    // config for the game receiver.
    public class GameReceiverConfig
    {
        public static GameReceiverConfig Default
        {
            get
            {
                GameReceiverConfig config = new GameReceiverConfig();
                config.Name = "New Game";
                config.MessageCooldownLength = 1;
                config.Topic = "A new game.";
                config.UpdateLastMessage = true;
                return config;
            }
        }
        private string _name;
        public string Name { get { return _name; } set { _name = OriFormat.FormatTextChannelName(value); } }

        public bool UpdateLastMessage { get; internal set; }
        public string Topic { get => Properties.Topic.Value; set => Properties.Topic = value; }
        public int MessageCooldownLength { get => Properties.SlowModeInterval.Value; set => Properties.SlowModeInterval = value; }

        public TextChannelProperties Properties { get; internal set; } = new TextChannelProperties();
    }
}
