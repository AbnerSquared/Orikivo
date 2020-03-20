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
                return new GameReceiverConfig
                {
                    Name = "New Game",
                    MessageCooldownLength = 1,
                    Topic = "A new game.",
                    CanUpdateMessage = true
                };
            }
        }

        private string _name;
        public string Name { get => _name; set => _name = OriFormat.TextChannelName(value); }

        public bool CanUpdateMessage { get; internal set; }
        public string Topic { get => Properties.Topic.Value; set => Properties.Topic = value; }
        public int MessageCooldownLength { get => Properties.SlowModeInterval.Value; set => Properties.SlowModeInterval = value; }

        public TextChannelProperties Properties { get; internal set; } = new TextChannelProperties();
    }
}
