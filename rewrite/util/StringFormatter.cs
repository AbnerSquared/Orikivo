using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// An object used to assist in creating formattable strings with arguments that can be passed into it.
    /// </summary>
    public class StringFormatter
    {
        public static StringFormatter LobbyInfoFormatter => new StringFormatter("> **{0}**#{1} `{2}`\n> ⇛ Playing **{3}**");
        public static StringFormatter ConsoleLineFormatter => new StringFormatter("[{0}]: \"{1}\"");
        public static StringFormatter UserTitleFormatter => new StringFormatter("Users (**{0}**/**{1}**):");

        public StringFormatter(string formatter, params string[] args)
        {
            Formatter = formatter;
            bool outOfRange = false;
            int i = 0;
            RequiredArgs = 0;
            while (!outOfRange)
            {
                if (!Formatter.Contains($"{{{i}}}"))
                    outOfRange = true;
                else
                {
                    i++;
                    RequiredArgs++;
                }
            }

            Args.Capacity = RequiredArgs;

            foreach (string arg in args)
                Args.Add(arg);
        }

        /// <summary>
        /// The string that is used to create the formatted string with.
        /// </summary>
        public string Formatter { get; }

        /// <summary>
        /// The amount of required arguments needed in order to format.
        /// </summary>
        public int RequiredArgs { get; }

        /// <summary>
        /// The collection of arguments to pass into the formatting string.
        /// </summary>
        public List<string> Args { get; } = new List<string>();

        public StringFormatter WithArgs(params string[] args)
        {
            Args.AddRange(args);
            return this;
        }
                
        /// <summary>
        /// The amount of arguments currently in the collection.
        /// </summary>
        public int ArgCount => Args?.Count ?? 0;

        /// <summary>
        /// Defines if the string can be formatted.
        /// </summary>
        public bool CanFormat => ArgCount == RequiredArgs;

        public override string ToString()
        {
            string result = Formatter;
            if (!CanFormat)
                throw new Exception("The amount of arguments compared to the required amount of arguments does not match.");
            for (int i = 0; i < RequiredArgs; i++)
            {
                if (!Formatter.Contains($"{{{i}}}"))
                    throw new Exception("There is a missing format fragment.");

                result = result.Replace($"{{{i}}}", Args[i]);
            }

            return result;            
        }
    }
}
