using Discord;

namespace Orikivo
{
    // instead of creating yet another service for the lobby to manage
    // you would create a separate display dedicated to the game.
    // in this case, if someone joins the lobby and the game is in progress, they can bring up the old lobby display, and focus on updating that
    // receivers would have to change their focus, maybe a targetDisplay, which would specify what display they read off of.
    // due to the new node organization, you could technically make targeted node classes for specific purposes.


    // config for the game.
    public class GameConfig
    {
        public GameConfig(GameMode mode, string name = null, GamePrivacy privacy = GamePrivacy.Local, string password = null, GameReceiverConfig receiverConfig = null)
        {
            Mode = mode;
            Privacy = privacy;
            Password = password;
            ReceiverConfig = receiverConfig ?? GameReceiverConfig.Default;
            if (Checks.NotNull(name))
                ReceiverConfig.Name = name;
        }

        public string Name { get; internal set; }
        public GamePrivacy Privacy { get; internal set; }
        public string Password { get; internal set; }
        public GameMode Mode { get; internal set; }

        public GameReceiverConfig ReceiverConfig { get; internal set; }
    }

    // config for the game receiver.
    public class GameReceiverConfig
    {
        public static GameReceiverConfig Default = new GameReceiverConfig { Name = "New Game", MessageCooldownLength = 1, Topic = "A new game.", UpdateLastMessage = true };
        private string _name;
        public string Name { get { return _name; } set { _name = OriFormat.FormatTextChannelName(value); } }

        public bool UpdateLastMessage { get; internal set; }
        public string Topic { get => Properties.Topic.Value; set => Properties.Topic = value; }
        public int MessageCooldownLength { get => Properties.SlowModeInterval.Value; set => Properties.SlowModeInterval = value; }

        public TextChannelProperties Properties { get; internal set; } = new TextChannelProperties();
    }
}
