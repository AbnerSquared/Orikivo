namespace Orikivo
{
    public class GameConfig
    {
        public GameConfig(GameMode mode, string name = null, GamePrivacy privacy = GamePrivacy.Local, string password = null, GameReceiverConfig receiverConfig = null)
        {
            Mode = mode;
            Privacy = privacy;
            Password = password;
            receiverConfig = receiverConfig ?? GameReceiverConfig.Default;
            if (Check.NotNull(name))
            {
                Name = name;
                receiverConfig.Name = name;
            }
            else
            {
                Name = "New Game";
            }

            ReceiverConfig = receiverConfig;
        }

        public string Name { get; internal set; }

        public GamePrivacy Privacy { get; internal set; }

        public string Password { get; internal set; }

        public GameMode Mode { get; internal set; }

        public GameReceiverConfig ReceiverConfig { get; internal set; }
    }
}
