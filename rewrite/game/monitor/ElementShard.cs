using System;

namespace Orikivo
{
    public class ElementShard
    {
        public static ElementShard LobbyInfoShard => new ElementShard("> **{0}**#{1} `{2}`\n> ⇛ Playing **{3}**");
        public static ElementShard ConsoleLineShard => new ElementShard("[{0}]: \"{1}\"");
        public static ElementShard UserTitleShard => new ElementShard("Users (**{0}**/**{1}**):");

        public ElementShard(string formatter)
        {
            Formatter = formatter;
            bool outOfRange = false;
            int i = 0;
            Count = 0;
            while (!outOfRange)
            {
                if (!Formatter.Contains($"{{{i}}}"))
                    outOfRange = true;
                else
                {
                    i++;
                    Count++;
                }
            }
        }

        public string Formatter { get; set; }
        public int Count { get; }// required strings to pass.
        public string ToString(params string[] args)
        {
            string result = Formatter;
            if (args.Length != Count)
                throw new Exception("You must pass the exact count this shard specifies.");
            for (int i = 0; i < Count; i++)
            {
                if (!Formatter.Contains($"{{{i}}}"))
                    throw new Exception("There is a missing format fragment.");

                result = result.Replace($"{{{i}}}", args[i]);
            }

            return result;            
        }
    }
}
