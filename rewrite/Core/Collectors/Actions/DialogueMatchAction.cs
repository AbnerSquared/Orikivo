using Discord;
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
    public class DialogueMatchAction : MatchAction
    {
        public DialogueMatchAction(OriCommandContext context)
        {
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
                    new Relationship
                    {
                        NpcId = "npc1",
                        Value = 0.2f
                    }
                },
                Sheet = new NpcSheet
                {
                    BodyFrame = new Sprite("../assets/npcs/noname/noname_body.png"),
                    HeadFrame = new Sprite("../assets/npcs/noname/noname_head.png"),
                    Reactions = new Dictionary<DialogueTone, Sprite>
                    {
                        [DialogueTone.Neutral] = new Sprite("../assets/npcs/noname/noname_neutral.png"),
                        [DialogueTone.Happy] = new Sprite("../assets/npcs/noname/noname_happy.png"),
                        [DialogueTone.Sad] = new Sprite("../assets/npcs/noname/noname_sad.png"),
                        [DialogueTone.Confused] = new Sprite("../assets/npcs/noname/noname_confused.png"),
                        [DialogueTone.Shocked] = new Sprite("../assets/npcs/noname/noname_shocked.png"),
                    }
                }
            };
            Pool = new DialoguePool
            {
                Entry = "Hello.",
                Dialogue = new List<Dialogue>
                {
                    new Dialogue
                    {
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
                        Id = "1",
                        Entries = new List<string>
                        {
                            "How are you?"
                        },
                        ReplyIds = new List<string>
                        {
                            "2"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
                        Id = "2",
                        Entries = new List<string>
                        {
                            "I'm okay. Thank you for asking."
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
                        Id = "3",
                        Entries = new List<string>
                        {
                            "how much wood can a woodchuck chuck if a woodchuck could chuck wood"
                        },
                        ReplyIds = new List<string>
                        {
                            "4"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Confused,
                        Id = "4",
                        Entries = new List<string>
                        {
                            "what does that even mean"
                        },
                        ReplyIds = new List<string>
                        {
                            "5"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
                        Id = "5",
                        Entries = new List<string>
                        {
                            "/shrug"
                        },
                        ReplyIds = new List<string>
                        {
                            "6"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
                        Id = "6",
                        Entries = new List<string>
                        {
                            "ok boomer"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
                        Id = "7",
                        Entries = new List<string>
                        {
                            "Do you like water?"
                        },
                        ReplyIds = new List<string>
                        {
                            "8",
                            "9"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Confused,
                        Id = "8",
                        Entries = new List<string>
                        {
                            "Considering we need it to live, yeah. Anything else?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Question,
                        Tone = DialogueTone.Happy,
                        Id = "9",
                        Entries = new List<string>
                        {
                            "Water is pretty epic. What about you?"
                        },
                        ReplyIds = new List<string>
                        {
                            "10",
                            "11"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
                        Id = "10",
                        Entries = new List<string>
                        {
                            "it be vibin"
                        },
                        ReplyIds = new List<string>
                        {
                            "12"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
                        Id = "11",
                        Entries = new List<string>
                        {
                            "I do not like it. Too water-y."
                        },
                        ReplyIds = new List<string>
                        {
                            "6"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Confused,
                        Id = "12",
                        Entries = new List<string>
                        {
                            "..."
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
                        Id = "13",
                        Entries = new List<string>
                        {
                            "Anything new?"
                        },
                        ReplyIds = new List<string>
                        {
                            "14",
                            "19"
                        }
                    },
                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
                        Id = "14",
                        Entries = new List<string>
                        {
                            "I met up with someone called a 'Pocket Lawyer'. They were the size of my palm, but got me out of debt."
                        },
                        ReplyIds = new List<string>
                        {
                            "15"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
                        Id = "15",
                        Entries = new List<string>
                        {
                            "woah wait what?"
                        },
                        ReplyIds = new List<string>
                        {
                            "16"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Happy,
                        Id = "16",
                        Entries = new List<string>
                        {
                            "Yeah, it was bizarre. ORS got nothin' on me now."
                        },
                        ReplyIds = new List<string>
                        {
                            "17"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
                        Id = "17",
                        Entries = new List<string>
                        {
                            "ORS?"
                        },
                        ReplyIds = new List<string>
                        {
                            "18"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Neutral,
                        Id = "18",
                        Entries = new List<string>
                        {
                            "I'd prefer if we don't delve into the topic. Anything else on your mind?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Shocked,
                        Id = "19",
                        Entries = new List<string>
                        {
                            "Did you know there's so much more to this world than the sector we live in? I hope I can go out there one day."
                        },
                        ReplyIds = new List<string>
                        {
                            "20"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Reply,
                        Tone = DialogueTone.Neutral,
                        Id = "20",
                        Entries = new List<string>
                        {
                            "I wish you luck."
                        },
                        ReplyIds = new List<string>
                        {
                            "21"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Answer,
                        Tone = DialogueTone.Happy,
                        Id = "21",
                        Entries = new List<string>
                        {
                            "Thanks. Anything else you wanted to talk about?"
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.Initial,
                        Tone = DialogueTone.Neutral,
                        Id = "22",
                        Entries = new List<string>
                        {
                            "I gotta go."
                        },
                        ReplyIds = new List<string>
                        {
                            "23"
                        }
                    },

                    new Dialogue
                    {
                        Type = DialogueType.End,
                        Tone = DialogueTone.Neutral,
                        Id = "23",
                        Entries = new List<string>
                        {
                            "See ya."
                        },
                        ReplyIds = new List<string>
                        {
                        }
                    }

                }
            };
        }

        public OriCommandContext Context { get; }

        public RestUserMessage InitialMessage { get; private set; }

        public DialoguePool Pool { get; }

        public Npc Npc { get; }

        // these are the next set of replies the user can use?
        public List<string> ResponseIds { get; private set; }

        public GammaPalette Palette { get; set; }

        public override async Task OnStartAsync()
        {
            StringBuilder chat = new StringBuilder();
            chat.AppendLine(Pool.Entry);

            chat.AppendJoin("\n", Pool.GetEntryTopics().Select(x => $"> {x.Id} - *\"{x.Entry}\"*"));

            ResponseIds = Pool.GetEntryTopics().Select(x => x.Id).ToList();

            await Context.Channel.SendImageAsync(Npc.Sheet.GetDisplayImage(DialogueTone.Neutral, Palette), "../tmp/npc.png");

            //InitialMessage.ModifyAsync(x => x.)
            InitialMessage = await Context.Channel.SendMessageAsync(chat.ToString());
        }

        private string GetNpcReplyBox(string response)
        {
            StringBuilder reply = new StringBuilder();

            reply.Append($"**{Npc.Name}**: {response}");
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

                chat.AppendLine(loop.NextEntry());

                if (loop.Type == DialogueType.End)
                {
                    await InitialMessage.ModifyAsync(x => x.Content = chat.ToString());

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
                    chat.AppendLine($"**No responses available. Closing...**");
                    await InitialMessage.ModifyAsync(x => x.Content = chat.ToString());
                    return ActionResult.Fail;
                }

                chat.AppendJoin("\n", ResponseIds.Select(x => $"> {x} - *\"{Pool.GetDialogue(x).Entry}\"*"));

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
            await InitialMessage.ModifyAsync(x => x.Content = "aite ima head out.");
        }
    }
}
