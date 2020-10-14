using Discord.WebSocket;
using Orikivo.Desync;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Orikivo.Drawing;
using Discord;
using Discord.Addons.Collectors;
using Orikivo.Canary;

namespace Orikivo
{

    /// <summary>
    /// Represents a dialogue session for a <see cref="Character"/>.
    /// </summary>
    public class ChatSession : MessageSession
    {
        public ChatSession(DesyncContext context, Character npc, DialogTree pool, PaletteType palette = PaletteType.Glass)
        {
            Context = context;
            Npc = npc;
            Tree = pool;
            Palette = GraphicsService.GetPalette(palette);
        }

        public DesyncContext Context { get; }

        private Husk Husk => Context.Account.Husk;

        private HuskBrain Brain => Context.Account.Brain;

        public IUserMessage MessageReference { get; private set; }

        public DialogTree Tree { get; }

        // TODO: Implement chat logging to handle future dialogue.
        public ChatLog Log { get; private set; } = new ChatLog();

        // TODO: Implement chat states
        public ChatState State { get; private set; }

        public Character Npc { get; }

        // these are the next set of replies the user can use?
        public List<string> ResponseIds { get; private set; }

        public GammaPalette Palette { get; set; }
        
        public AffinityData Affinity { get; set; }

        private DialogTone LastTone { get; set; } = DialogTone.Neutral;

        private static readonly string _replyFrame = "> `{0}` • *\"{1}\"*";

        private IEnumerable<string> WriteAvailableReplies()
            => ResponseIds.Select(x => string.Format(_replyFrame, x, Tree.Branches.First().GetDialog(x).Entry.ToString()));

        private string GetReplyBox(string response, bool showReplies = true)
        {
            var reply = new StringBuilder();

            reply.AppendLine($"**{Npc.Name}**: {response}");

            if (showReplies)
                reply.AppendJoin("\n", WriteAvailableReplies());

            return reply.ToString();
        }

        // TODO: Separate branches into their own categories
        // For now, only use the first branch
        private List<string> GetEntryIds()
            => Tree.Branches.First().GetEntryDialogs(Npc, Husk, Brain, Log).Select(x => x.Id).ToList();

        public override async Task OnStartAsync()
        {
            ResponseIds = GetEntryIds();
            Affinity = Context.Account.Brain.GetOrAddAffinity(Npc);

            if (Npc.Model != null)
                MessageReference = await Context.Channel.SendImageAsync(Npc.Model.Render(DialogTone.Neutral, Palette), "../tmp/npc.png", GetReplyBox("Hello."));
            else // TODO: Implement random greetings.
                MessageReference = await Context.Channel.SendMessageAsync(GetReplyBox("Hello."));
        }

        public override async Task<SessionResult> OnMessageReceivedAsync(SocketMessage message)
        {
            if (ResponseIds.Contains(message.Content))
            {
                var chat = new StringBuilder();

                Dialog response = Tree.Branches.First().GetDialog(message.Content);
                Dialog loop = Tree.Branches.First().GetBestReply(Npc, Husk, Brain, Log, response);

                switch (loop.Type)
                {
                    case DialogType.End:
                        // TODO: Implement content separations, which are continued when the user types 'next' (loop.GetBestEntry(Npc))
                        await UpdateMessageAsync(loop.Tone, GetReplyBox(loop.GetAnyEntry().ToString(), false));
                        return SessionResult.Success;

                    case DialogType.Answer:
                        ResponseIds = GetEntryIds();
                        break;

                    default:
                        if (loop.ReplyIds.Count == 0)
                        {
                            chat.AppendLine($"> **No responses available. Closing...**");
                            await UpdateMessageAsync(loop.Tone, chat.ToString());
                            return SessionResult.Fail;
                        }

                        ResponseIds = loop.ReplyIds;
                        break;
                }

                chat.AppendLine(GetReplyBox(loop.GetAnyEntry().ToString()));

                await UpdateMessageAsync(loop.Tone, chat.ToString());

                return SessionResult.Continue;
            }
            else
            {
                string old = MessageReference.Content;

                await MessageReference.ModifyAsync($"> **Please input a correct response ID.**\n" + old);
                return SessionResult.Continue;
            }
        }

        private async Task UpdateMessageAsync(DialogTone tone, string chatBox)
        {
            // TODO: Implement deleting messages if a bot has permission to

            if (Npc.Model != null && LastTone != tone)
            {
                MessageReference = await MessageReference.ReplaceAsync(Npc.Model.Render(tone, Palette), "../tmp/npc.png", chatBox, deleteLastMessage: true);
                LastTone = tone;
            }
            else
            {
                await MessageReference.ModifyAsync(chatBox);
            }
        }

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            // TODO: Implement random OnTimeout strings
            await MessageReference.ModifyAsync(GetReplyBox(Tree.OnTimeout, false));
        }
    }
}
