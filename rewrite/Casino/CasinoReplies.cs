namespace Orikivo.Casino
{
    // TODO: Create criteria-based replies.
    public static class CasinoReplies
    {
        public static string DebtGeneric = "You have been fined.";
        public static string CurseGeneric = "You have been cursed.";
        public static string GoldGeneric = "You have been blessed.";
        public static string WinGeneric = "You have won.";
        public static string LoseGeneric = "You have lost.";
        public static string EvenGeneric = "You broke even.";

        public static string NextReply(GimiResultFlag flag)
        {
            string[] replies = GetReplies(flag);
            string generic = GetGeneric(flag);

            return replies.Length > 0 ? Randomizer.Choose(replies) : generic;
        }

        private static string GetGeneric(GimiResultFlag flag)
            => flag switch
            {
                GimiResultFlag.Win => WinGeneric,
                GimiResultFlag.Gold => GoldGeneric,
                GimiResultFlag.Lose => LoseGeneric,
                GimiResultFlag.Curse => CurseGeneric,
                _ => throw new System.Exception("Unknown flag.")
            };

        private static string[] GetReplies(GimiResultFlag flag)
            => flag switch
            {
                GimiResultFlag.Win => Win,
                GimiResultFlag.Gold => Gold,
                GimiResultFlag.Lose => Lose,
                GimiResultFlag.Curse => Curse,
                _ => throw new System.Exception("Unknown flag.")
            };



        public static string[] Debt =
        {

        };

        public static string[] Curse =
        {
            "Experience oblivion.",
        };

        public static string[] Gold =
        {
            "All that truly glitters is gold.",
            "The calm before the storm."
        };

        public static string[] Win =
        {
            "A little bit of **Orite** can go far in life.",
            "Your wish has been granted.",
            "Hope is a powerful emotion.",
            "Ask and you shall receive."
        };

        public static string[] Lose =
        {
            "They can't all be winners.",
            "Yikes!"
        };
    }
}
