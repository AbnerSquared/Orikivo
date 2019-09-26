using Discord;

namespace Orikivo.Static
{
    /// <summary>
    /// Manages a collection of Emojis that Orikivo uses.
    /// </summary>
    public static class EmojiIndex
    {
        public static Emoji Balance = new Emoji("💸");
        public static Emoji Expended = new Emoji("📉");
        public static Emoji MostHeld = new Emoji("💰");
        public static Emoji Midas = new Emoji("🉑");
        public static Emoji Experience = new Emoji("🔺");
        public static Emoji GoldMedal = new Emoji("🥇");
        public static Emoji SilverMedal = new Emoji("🥈");
        public static Emoji BronzeMedal = new Emoji("🥉");
        public static Emoji Unread = new Emoji("🔹");
        public static Emoji Mail = new Emoji("📧");
        public static Emoji EmptyMailbox = new Emoji("📭");
        public static Emoji InboundMail = new Emoji("📬");
        public static Emoji Locked = new Emoji("🔒");
        public static Emoji Debt = new Emoji("📝");
        public static Emoji Ping = new Emoji("🏓");
        public static Emoji VisualFlag = new Emoji("💬");
        public static Emoji PriorityFlag = new Emoji("❌");
        public static Emoji SpeedFlag = new Emoji("🐌");
        public static Emoji ExceptionFlag = new Emoji("⚡");
        public static Emoji SuggestFlag = new Emoji("💡");
        public static Emoji CrossChat = new Emoji("📨");
        public static Emoji Success = new Emoji("✅");
        public static Emoji Active = new Emoji("🔵");
        public static Emoji Inactive = new Emoji("🔸");
        public static Emoji Dice = new Emoji("🎲");
        public static Emoji Coin = new Emoji("⚫");
        public static Emoji Heads = new Emoji("⚫");
        public static Emoji Tails = new Emoji("⚪");
        public static Emoji Tick = new Emoji("⏰");
        public static Emoji Slots = new Emoji("🎰");
        public static Emoji Bot = new Emoji("📡");
        public static Emoji Webhook = new Emoji("🔗");
        public static Emoji Playing = new Emoji("🎮");
        public static Emoji Listening = new Emoji("🎧");
        public static Emoji Watching = new Emoji("📺");
        public static Emoji Streaming = new Emoji("📹");
        public static Emoji Online = new Emoji("💚");
        public static Emoji Away = new Emoji("💛");
        public static Emoji Busy = new Emoji("❤");
        public static Emoji Offline = new Emoji("🖤");
        public static Emoji Pin = new Emoji("📌");
        public static Emoji Deaf = new Emoji("📣");
        public static Emoji Muted = new Emoji("🎧");
        public static Emoji Suppressed = new Emoji("💤");
        public static Emoji Image = new Emoji("📷");
        public static Emoji Identifier = new Emoji("🆔");
        public static Emoji Agenda = new Emoji("📁");
        public static Emoji Animated = new Emoji("🌠"); // file pager
        public static Emoji Owner = new Emoji("⚒");
        public static Emoji Color = new Emoji("🎨");
        public static Emoji Time = new Emoji("🕘");
        public static Emoji Hex = new Emoji("#⃣");
        public static Emoji SlowMode = new Emoji("⏰");
        public static Emoji Nsfw = new Emoji("🌶");
        public static Emoji Topic = new Emoji("📋");
        public static Emoji Category = new Emoji("📂"); // Symbolizes a category.
        public static Emoji Permissions = new Emoji("🔖"); // Symbolizes a permission collection.
        public static Emoji Permission = new Emoji("📄"); // Symbolizes a single permission.
        public static Emoji RawValue = new Emoji("🍖"); // Used to display a raw value of something, such as a permission value.
        public static Emoji Counter = new Emoji("⌚"); // Used for counters, such as member count.
        public static Emoji User = new Emoji("👤"); // Represents a user.
        public static Emoji Verification = new Emoji("🔐");
        public static Emoji Hoisted = new Emoji("⛓"); // Represents if a user will be raised up.
        public static Emoji Bitrate = new Emoji("📊"); // represents bitrate.
        public static Emoji Events = new Emoji("🗓"); // spiral calandar, signifies events
        public static Emoji Region = new Emoji("🌸");
        public static Emoji Role = new Emoji("💎"); // signifies status
        public static Emoji Text = new Emoji("💬"); // text channels.
        public static Emoji Voice = new Emoji("📣");
        public static Emoji Prefix = new Emoji("📞"); // signifies 
        public static Emoji Size = new Emoji("📐"); // signifies measurement of an item
        public static Emoji Report = new Emoji("📜");
        public static Emoji Channel = new Emoji("📻"); // represents a channel.
        public static Emoji TellerMachine = new Emoji("🏧");

        // time
        public static Emoji Clock1 = new Emoji("🕐"); // 1:00
        public static Emoji Clock2 = new Emoji("🕑"); // 2:00
        public static Emoji Clock3 = new Emoji("🕒"); // 3:00
        public static Emoji Clock4 = new Emoji("🕓"); // 4:00
        public static Emoji Clock5 = new Emoji("🕔"); // 5:00
        public static Emoji Clock6 = new Emoji("🕕"); // 6:00
        public static Emoji Clock7 = new Emoji("🕖"); // 7:00
        public static Emoji Clock8 = new Emoji("🕗"); // 8:00
        public static Emoji Clock9 = new Emoji("🕘"); // 9:00
        public static Emoji Clock10 = new Emoji("🕙"); // 10:00
        public static Emoji Clock11 = new Emoji("🕚"); // 11:00
        public static Emoji Clock12 = new Emoji("🕛"); // 12:00

        // methods to get an emoji from objects.
        public static Emoji FromHours(int hours)
        {
            Emoji clock = Clock12;

            if (hours.EqualsAny(11, 23)) clock = Clock11;
            else if (hours.EqualsAny(10, 22)) clock = Clock10;
            else if (hours.EqualsAny(9, 21)) clock = Clock9;
            else if (hours.EqualsAny(8, 20)) clock = Clock8;
            else if (hours.EqualsAny(7, 19)) clock = Clock7;
            else if (hours.EqualsAny(6, 18)) clock = Clock6;
            else if (hours.EqualsAny(5, 17)) clock = Clock5;
            else if (hours.EqualsAny(4, 16)) clock = Clock4;
            else if (hours.EqualsAny(3, 15)) clock = Clock3;
            else if (hours.EqualsAny(2, 14)) clock = Clock2;
            else if (hours.EqualsAny(1, 13)) clock = Clock1;

            return clock;
        }
    }
}