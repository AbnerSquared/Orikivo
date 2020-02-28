using Discord;

namespace Orikivo
{
    public class ReceiverConfig
    {
        public ReceiverConfig()
        {
            Name = "lobby";
            Properties = DefaultTextProperties;
            ModifySource = true;
        }

        public ReceiverConfig(string name, TextChannelProperties properties, bool modifySource = true)
        {
            Name = name;
            Properties = properties ?? DefaultTextProperties;
            ModifySource = modifySource;
        }

        public ReceiverConfig(string name, string topic = null, int slowModeInterval = 1, bool modifySource = true)
        {
            Name = name;
            Properties = new TextChannelProperties
            {
                Topic = topic,
                SlowModeInterval = slowModeInterval
            };
            ModifySource = modifySource;
        }

        public static TextChannelProperties DefaultTextProperties = new TextChannelProperties { Topic = "Game.Empty", SlowModeInterval = 1 };

        private string _name;
        public string Name { get { return _name; } set { _name = OriFormat.FormatTextChannelName(value); } }

        // if enabled, the receiver will depend only on one message
        // otherwise, the receiver will send a new message for each update
        public bool ModifySource { get; set; }

        public TextChannelProperties Properties { get; set; }
        public string Topic { get { return Properties?.Topic.GetValueOrDefault(); } set { Properties.Topic = value; } }
        public int SlowModeInterval { get { return Properties?.SlowModeInterval.GetValueOrDefault() ?? 0; } set { Properties.SlowModeInterval = value; } }
    }
}
