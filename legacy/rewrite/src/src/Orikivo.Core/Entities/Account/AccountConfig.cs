namespace Orikivo
{
    public enum IconFormat
    {
        Portable = 0,
        Escaped = 1,
        Packed = 2,
        Hidden = 4
    } // Text = 8 ? 

    public enum NullHandlerType
    {
        Include = 0,
        Hidden = 1
    }

    public class AccountConfig
    {
        public AccountConfig()
        {
            Nickname = null;
            Overflow = false;
            InsultLevel = InsultType.Polite;
            InboundMail = true;
            InboundUpdates = true;
        }
        public bool AutoCorrect { get; set; }
        public bool Overflow { get; set; } // Toggles the ability to proceed payments if the amount is over the current.
        public IconFormat IconStyle { get; set; }
        public bool Throw { get; set; }
        public NullHandlerType OnEmpty { get; set; }

        public string Nickname {get; set;} = null;
        public bool Mobile { get; set; } = false;
        public bool VerboseGimme { get; set; } = false;
        public bool InboundMail { get; set; } = true;
        public bool InboundUpdates { get; set; } = true; // toggles the ability to notify others in the case of a new update.
        public bool ProfileCard { get; set; } = true; // The check to see if you want a dynamic pixel card.
        public InsultType InsultLevel { get; set; } = InsultType.Polite; // The fallback to an insult command if a server-wide one is not set.
        //public int TransactionCapacity { get; set; } = 5; // the limit of transaction it saves, up to 25 at most.

        public void ToggleVerboseGimme()
            => VerboseGimme = !VerboseGimme;

        public void ToggleMobile()
            => Mobile = !Mobile;

        public void ToggleMail()
            => InboundMail = !InboundMail;

        public void ToggleUpdates()
            => InboundUpdates = !InboundUpdates;

        public void SetNickname(string s  = null)
        {
            Nickname = s;
        }

        public void ToggleOverflow()
        {
            Overflow = !Overflow;
        }

        public string GetInsultSummary()
        {
            switch (InsultLevel)
            {
                case InsultType.Adequate:
                    return "I'll try my best to... insult?";
                case InsultType.Brash:
                    return "I'm the second grader calling you stupid.";
                case InsultType.Rude:
                    return "I didn't have any coffee today.";
                case InsultType.Cruel:
                    return "The everyday jackwagon, now portable.";
                case InsultType.Painful:
                    return "I'm the toothpick in your nail when you kick the wall.";
                case InsultType.Flaming:
                    return "i m b u r n i n g";
                case InsultType.Help:
                    return $"a a  a   a    {"a".ToSubscript()}";
                default:
                    return "I actually can't be rude.";
            }
        }
    }
}