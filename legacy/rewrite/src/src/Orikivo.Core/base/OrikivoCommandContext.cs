using Discord.WebSocket;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Collections.Generic;
using Orikivo.Modules;

namespace Orikivo
{
    public class OrikivoCommandContext : SocketCommandContext
    {
        public OldAccount Account { get; }
        public Server Server { get; }
        public DataContainer Data { get; }
        public OldGlobal Global { get; }

        public SocketCategoryChannel Category { get; }

        public OrikivoCommandContext(DiscordSocketClient client, DataContainer data, SocketUserMessage msg) : base(client, msg)
        {
            if (Guild.TryGetTextChannel(msg.Channel.Id, out SocketTextChannel t))
            {
                if (t.Category.Exists())
                {
                    Category = t.Category as SocketCategoryChannel;
                }
            }

            Data = data;
            Global = Data.Global;

            if (Guild.Exists())
                Server = Data.GetOrAddServer(Guild);
            
            if (User.Exists())
            {
                if (Data.TryGetAccount(User, out OldAccount _a))
                {
                    Account = _a;
                    Account.Username = User.Username;
                }
            }
        }

        // Clean this up. Commands don't need to be as verbose as they should.
        public void RecordContext(CommandInfo command)
        {
            if (Account.Analytics is null)
                Account.Analytics = new AnalyzerOld();
            AnalyzerOld analyzer = Account.Analytics;
            analyzer.Debug();

            CommandAnalyzer tmp = analyzer.Search(command).Refresh(Message);
            analyzer.AnalyzeCommand(command, tmp);
            Data.Update(Account);
        }
    }
}