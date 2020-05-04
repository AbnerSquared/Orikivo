using Discord.WebSocket;
using Orikivo.Desync;
using System.Threading.Tasks;

using System.Collections.Generic;
using Discord.Rest;
using System.Text;
using System.Linq;
using Orikivo.Drawing;
using Discord;

namespace Orikivo
{
    public enum ChatState
    {
        Entry, // when you first start talking to an npc
        Speak, // when you are currently speaking to an npc
        Trade, // when you are willing to trade with an npc
        Gift, // when you are gifting to an npc
        Request, // when you are getting a request from an npc
        Give // when you are giving to an npc
    }
    /// <summary>
    /// Represents a custom dialogue session for an <see cref="Desync.Npc"/>.
    /// </summary>
    public class ChatHandler : MatchAction
    {
        public ChatHandler(OriCommandContext context, Npc npc, DialoguePool pool, PaletteType palette = PaletteType.Glass)
        {
            Context = context;
            Npc = npc;
            Pool = pool;
            Palette = GraphicsService.GetPalette(palette);
        }

        public OriCommandContext Context { get; }


        private Husk Husk => Context.Account.Husk;

        private HuskBrain Brain => Context.Account.Brain;

        public IUserMessage MessageReference { get; private set; }

        public DialoguePool Pool { get; }

        // TODO: Implement chat logging to handle future dialogue.
        public ChatLog Log { get; private set; }

        public Npc Npc { get; }

        // these are the next set of replies the user can use?
        public List<string> ResponseIds { get; private set; }

        public GammaPalette Palette { get; set; }
        
        public AffinityData Relationship { get; set; }

        public override async Task OnStartAsync()
        {

            ResponseIds = Pool.GetEntryTopics().Select(x => x.Id).ToList();
            Relationship = Context.Account.Brain.GetOrAddAffinity(Npc);

            // only if a sheet is supplied, should it be drawn.
            if (Npc.Model != null)
                MessageReference = await Context.Channel.SendImageAsync(Npc.Model.Render(DialogTone.Neutral, Palette), "../tmp/npc.png", GetReplyBox(Pool.Entry));
            else
                MessageReference = await Context.Channel.SendMessageAsync(GetReplyBox(Pool.Entry));
        }

        private string GetReplyBox(string response, bool showReplies = true)
        {
            StringBuilder reply = new StringBuilder();

            reply.AppendLine($"**{Npc.Name}**: {response}");

            if (showReplies)
                reply.AppendJoin("\n", ResponseIds.Select(x => $"> `{x}` • *\"{Pool.GetDialogue(x).Entry}\"*"));

            return reply.ToString();
        }

        public override async Task<ActionResult> InvokeAsync(SocketMessage message)
        {
            if (ResponseIds.Contains(message.Content))
            {
                StringBuilder chat = new StringBuilder();

                Dialogue response = Pool.GetDialogue(message.Content);
                Dialogue loop = Pool.GetDialogue(response.GetBestReplyId(Npc.Personality));

                if (loop.Type == DialogType.End)
                {
                    await MessageReference.ModifyAsync(x => x.Content = GetReplyBox(loop.NextEntry(), false));
                    return ActionResult.Success;
                }

                if (loop.Type == DialogType.Answer)
                {
                    ResponseIds = Pool.GetEntryTopics().Select(x => x.Id).ToList();
                }
                else if (loop.ReplyIds.Count > 0)
                {
                    ResponseIds = loop.ReplyIds;
                }
                else
                {
                    chat.AppendLine($"> **No responses available. Closing...**");
                    await MessageReference.ModifyAsync(x => x.Content = chat.ToString());
                    return ActionResult.Fail;
                }

                chat.AppendLine(GetReplyBox(loop.NextEntry()));

                await message.DeleteAsync();

                //await Context.Channel.SendImageAsync(Npc.Sheet.GetDisplayImage(loop.Tone, Palette), "../tmp/npc.png");
                await MessageReference.ModifyAsync(x => x.Content = chat.ToString());
                
                return ActionResult.Continue;
            }
            else
            {
                string old = MessageReference.Content;

                await MessageReference.ModifyAsync(x => x.Content = $"> **Please input a correct response ID.**\n" + old);
                return ActionResult.Continue;
            }
        }

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            await MessageReference.ModifyAsync(x => x.Content = GetReplyBox(Pool.Timeout, false));
        }
    }
}
