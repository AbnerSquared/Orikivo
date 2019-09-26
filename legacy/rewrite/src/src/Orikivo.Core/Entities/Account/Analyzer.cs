using Discord;
using Discord.Commands;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public class AnalyzerOld
    {
        public AnalyzerOld() {}

        /// <summary>
        /// Holds the total amount of money this account has spent.
        /// </summary>
        public ulong Expended { get; set; } = 0;

        /// <summary>
        /// Holds the max amount of money that was held in a wallet.
        /// </summary>
        public ulong MaxHeld { get; set; } = 0;

        public bool IsOverMaxHeld(ulong amt)
            => amt > MaxHeld;

        public void TryUpdateMaxHeld(ulong amt)
            => MaxHeld = IsOverMaxHeld(amt) ? amt : MaxHeld;

        public void UpdateExpended(ulong amt)
            => Expended += amt;

        public ulong Interactivity { get; set; } = 0;
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        // last queued songs....? 
        //public Dictionary<string, CommandAnalyzer> Commands {get; set;} = new Dictionary<string, CommandAnalyzer>();
        //public Dictionary<ulong, GameAnalyzer> Games {get; set;} = new Dictionary<ulong, GameAnalyzer>();

        //public void AnalyzeCommand(CommandInfo cmd, CommandAnalyzer analysis)
     //   {
          //  string builder = ($"{cmd.Module.Name ?? cmd.Module.ToString()}.{cmd.Name}").ToLower().Replace(" ", "-");
          //  var exists = Commands.TryAdd(builder, analysis);
         //   if (!exists)
          //  {
           //     Commands[builder] = analysis;
            //}
        //}

            /*
        public CommandAnalyzer Search(CommandInfo cmd)
        {
            if (!Commands.Exists()) { Commands = new Dictionary<string, CommandAnalyzer>(); }
            string builder = ($"{cmd.Module.Name ?? cmd.Module.ToString()}.{cmd.Name}").ToLower().Replace(" ", "-");
            bool found = Commands.TryGetValue(builder, out CommandAnalyzer analysis);
            return found ? analysis : new CommandAnalyzer();
        }*/

        //public override string ToString()
         //   => $"{Commands.Count} commands analyzed.";
        
    }
}