using Discord.WebSocket;
using Orikivo.Unstable;
using System.Threading.Tasks;

using System.Collections.Generic;
using Discord.Rest;
using System.Text;
using System.Linq;
using Orikivo.Drawing;

namespace Orikivo
{

    /// <summary>
    /// Represents a custom dialogue session for an <see cref="Unstable.Npc"/>.
    /// </summary>
    public class ChatHandler : MatchAction
    {
        public ChatHandler(OriCommandContext context)
        {
            SpriteBank bank = SpriteBank.FromDirectory("../assets/npcs/noname/");
            Context = context;
            Palette = GammaPalette.Glass;
            Npc = new Npc
            {
                Id = "npc0",
                Name = "No-Name",
                Personality = new Personality
                {
                    Archetype = Archetype.Generic
                },
                Relations = new List<Relationship>
                {
                    new Relationship("npc1", 0.2f)
                },
                Sheet = new NpcSheet
                {
                    Body = bank.GetSprite("noname_body"),
                    BodyOffset = new System.Drawing.Point(20, 16),
                    Head = bank.GetSprite("noname_head"),
                    HeadOffset = new System.Drawing.Point(28, 5),
                    FaceOffset = new System.Drawing.Point(28, 5),
                    Reactions = new Dictionary<DialogueTone, Sprite>
                    {
                        [DialogueTone.Neutral] = bank.GetSprite("noname_neutral"),
                        [DialogueTone.Happy] = bank.GetSprite("noname_happy"),
                        [DialogueTone.Sad] = bank.GetSprite("noname_sad"),
                        [DialogueTone.Confused] = bank.GetSprite("noname_confused"),
                        [DialogueTone.Shocked] = bank.GetSprite("noname_shocked"),
                    }
                }
            };
            Pool = WorldEngine.GetPool("test");
        }

        public ChatHandler(OriCommandContext context, Npc npc, DialoguePool pool, PaletteType palette = PaletteType.Glass)
        {
            Context = context;
            Npc = npc;
            Pool = pool;
            Palette = GraphicsService.GetPalette(palette);
        }

        public OriCommandContext Context { get; }

        public RestUserMessage InitialMessage { get; private set; }

        public DialoguePool Pool { get; }

        // TODO: Implement chat logging to handle future dialogue.
        public ChatLog Log { get; private set; }

        public Npc Npc { get; }

        // these are the next set of replies the user can use?
        public List<string> ResponseIds { get; private set; }

        public GammaPalette Palette { get; set; }
        
        public Relationship Relationship { get; set; }

        public override async Task OnStartAsync()
        {

            ResponseIds = Pool.GetEntryTopics().Select(x => x.Id).ToList();
            Relationship = Context.Account.Brain.GetOrCreateRelationship(Npc);

            // only if a sheet is supplied, should it be drawn.
            if (Npc.Sheet != null)
                InitialMessage = await Context.Channel.SendImageAsync(Npc.Sheet.GetDisplayImage(DialogueTone.Neutral, Palette), "../tmp/npc.png", GetReplyBox(Pool.Entry));
            else
                InitialMessage = await Context.Channel.SendMessageAsync(GetReplyBox(Pool.Entry));
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
                Dialogue loop = Pool.GetDialogue(response.GetBestReplyId(Npc.Personality.Archetype));

                if (loop.Type == DialogueType.End)
                {
                    await InitialMessage.ModifyAsync(x => x.Content = GetReplyBox(loop.NextEntry(), false));
                    return ActionResult.Success;
                }

                if (loop.Type == DialogueType.Answer)
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
                    await InitialMessage.ModifyAsync(x => x.Content = chat.ToString());
                    return ActionResult.Fail;
                }

                chat.AppendLine(GetReplyBox(loop.NextEntry()));

                await message.DeleteAsync();

                //await Context.Channel.SendImageAsync(Npc.Sheet.GetDisplayImage(loop.Tone, Palette), "../tmp/npc.png");
                await InitialMessage.ModifyAsync(x => x.Content = chat.ToString());
                
                return ActionResult.Continue;
            }
            else
            {
                string old = InitialMessage.Content;

                await InitialMessage.ModifyAsync(x => x.Content = $"> **Please input a correct response ID.**\n" + old);
                return ActionResult.Continue;
            }
        }

        public override async Task OnTimeoutAsync(SocketMessage message)
        {
            await InitialMessage.ModifyAsync(x => x.Content = GetReplyBox(Pool.Timeout, false));
        }
    }
}
