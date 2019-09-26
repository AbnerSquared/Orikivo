using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using Orikivo.Static;
using System.Threading.Tasks;
using Orikivo.Systems.Presets;
using Microsoft.Extensions.Configuration;
using Discord.Rest;
using System.Diagnostics;

namespace Orikivo.Modules
{
    [Name("Interaction")]
    [Summary("Communication made easy in a variety of ways.")]
    [DontAutoLoad]
    public class InteractionModule : ModuleBase<OrikivoCommandContext>
    {
        public async Task SendMessageAsync(string content)
        {
            await ReplyAsync(content);
        }

        // give others the ability to join servers met by crosschat
        public async Task CrossChatAsync(Server server, SocketGuild guild, string content)
        {
            if (!server.Config.CrossChat)
            {
                await ReplyAsync("**CrossChat** is disabled in your server.");
                return;
            }

            Server s = Context.Data.GetOrAddServer(guild);
            if (!s.Config.CrossChat)
            {
                await ReplyAsync($"{guild.Name} has excluded themselves from **CrossChat** communication.");
                return;
            }

            SocketGuild g = Context.Client.GetGuild(s.Id);
            SocketTextChannel c = Context.Client.GetChannel(s.Config.InboundChannel) as SocketTextChannel;

            if (!c.Exists())
            {
                if (!Context.TryGetPrimaryChatChannel(g, out c))
                {
                    await ReplyAsync($"{g.Name} is lacking an available channel to relay. Try nudging the owner.");
                    return;
                }
            }

            EmbedBuilder box = GenerateChatBox(Context.Account, s, g, content);
            await c.TriggerTypingAsync();
            await c.SendMessageAsync(embed: box.Build());
            await Context.Message.AddReactionAsync(EmojiIndex.Success);
        }

        public EmbedBuilder GenerateChatBox(OldAccount a, Server s, SocketGuild g, string content)
        {
            string command = "message";
            List<string> demobl = new List<string> { "darn" };
            List<string> blacklist = Context.Data.Blacklist.Merge(s.Config.Blacklist, demobl);
            EmbedBuilder e = new EmbedBuilder();
            EmbedFooterBuilder f = new EmbedFooterBuilder();

            f.WithText($"{s.Config.GetPrefix(Context)}{command} {Context.Guild.Id} <{nameof(content)}>");

            e.WithColor(EmbedData.GetColor("origreen"));
            e.WithDescription(s.Config.SafeChat ? content.Guard(blacklist).Escape('*') : content);
            e.WithTitle($"{EmojiIndex.CrossChat.Pack(a)} {a.GetName()}");
            e.WithFooter(f);

            return e;
        }

        public async Task GetInboundChannelAsync(SocketGuild g, Server s)
        {
            if (s.Config.InboundChannel == 0)
            {
                Embed e = EmbedData.Throw(Context, $"**CrossChat** is currently unbound to a channel.");
                await ReplyAsync(embed: e);
                return;
            }

            if (!g.TryGetTextChannel(s.Config.InboundChannel, out SocketTextChannel c))
            {
                Embed e = EmbedData.Throw(Context, $"{g.Name} is lacking the saved channel.", "This channel no longer exists, and the inbound channel will now be reset.", false);
                await ReplyAsync(embed: e);
                // find a default value.
                s.Config.InboundChannel = 0;
                return;
            }

            EmbedBuilder emb = EmbedData.DefaultEmbed;
            emb.WithColor(EmbedData.GetColor("error"));
            emb.WithDescription($"**CrossChat** is currently bound to {c.Mention}.");
            
            await ReplyAsync(embed: emb.Build());
        }

        public async Task ToggleWordGuardAsync(SocketGuildUser u, SocketGuild g, Server s)
        {
            if (!u.EnsureRank(s, g))
            {
                await ReplyAsync("You must be ensured to configure **WordGuard**.");
                return;
            }

            s.Config.ToggleSafeChat();

            await ReplyAsync($"{(s.Config.SafeChat ? "**WordGuard** has been enabled." : "**WordGuard** has been disabled.")}");
            Context.Data.Update(s);
        }

        public async Task ToggleCrossChatAsync(SocketGuildUser u, SocketGuild g, Server s)
        {
            if (!u.EnsureRank(s, g))
            {
                await ReplyAsync("You must be ensured to configure **CrossChat**.");
                return;
            }

            s.Config.ToggleCrossChat();

            await ReplyAsync($"{(s.Config.CrossChat ? "**CrossChat** has been enabled." : "**CrossChat** has been disabled.")}");
            Context.Data.Update(s);
        }

        public async Task ViewCrossChatAsync(Server s)
        {
            SocketGuild g = s.Guild(Context.Client);
            EmbedBuilder e = EmbedData.DefaultEmbed;


            StringBuilder sb = new StringBuilder();
            sb.Append(s.Config.CrossChat ? "**CrossChat** is active" : "**CrossChat** is currently disabled.");

            if (s.Config.CrossChat)
            {
                bool hasDefault = Context.TryGetPrimaryChatChannel(g, out SocketTextChannel def);
                if (s.Config.InboundChannel > 0)
                {
                    if (!g.TryGetTextChannel(s.Config.InboundChannel, out SocketTextChannel inb))
                    {
                        sb.AppendLine(", but is missing the inbound channel.");
                        if (hasDefault)
                        {
                            sb.AppendLine($"All messages will be sent to {def.Mention}.");
                        }
                        else
                        {
                            sb.AppendLine("No messages can be received at this time, due to the lack of a default chat channel.");
                        }
                    }
                    else
                    {
                        sb.AppendLine($", and is bound to {inb.Mention}.");
                    }
                }
                else
                {
                    sb.AppendLine(", but is currently unbound to a channel.");
                    if (hasDefault)
                    {
                        sb.AppendLine($"All messages will be sent to {def.Mention}.");
                    }
                    else
                    {
                        sb.AppendLine("No messages can be received at this time, due to the lack of a default chat channel.");
                    }
                }
            }
            else
            {
                e.WithColor(EmbedData.GetColor("error"));
            }

            e.WithDescription(sb.ToString());
            await ReplyAsync(embed: e.Build());
        }

        public async Task SetInboundChannelAsync(SocketGuildUser u, Server s, SocketGuild g, SocketTextChannel c)
        {
            if (!u.EnsureRank(s, g))
            {
                await ReplyAsync(embed: EmbedData.Throw(Context, "Unensured.", "You must be ensured to configure **CrossChat's** inbound channel.", false));
                return;
            }

            EmbedBuilder e = EmbedData.DefaultEmbed;
            e.WithTitle("Inbound channel set.");
            e.WithDescription($"Inbound messages will now be sent to {c.Mention}.");

            s.Config.SetInboundChannel(c);
            await ReplyAsync(embed: e.Build());
            Context.Data.Update(s);
        }

        [Command("tts")]
        [Summary("eeeeeeeeeeeeeeee")]
        public async Task TextToSpeech([Remainder]string message)
        {
            await ModuleManager.TryExecute(Context.Channel, ReplyAsync(message, true));
        }

        [Command("inboundchannel"), Priority(0)]
        [Summary("View your crosschat channel location.")]
        public async Task SetInboundChannelResponseAsync()
        {
            await ModuleManager.TryExecute(Context.Channel, GetInboundChannelAsync(Context.Guild, Context.Server));
        }

        [Command("inboundchannel"), Priority(1)]
        [Summary("Allows you to set your crosschat channel location.")]
        public async Task SetInboundChannelResponseAsync(SocketTextChannel channel)
        {
            await ModuleManager.TryExecute(Context.Channel, SetInboundChannelAsync(Context.GetGuildUser(), Context.Server, Context.Guild, channel));
        }

        //[Command("wordguard"), Alias("wg")]
        //[Summary("View WordGuard information.")]
        public async Task ViewWordGuardResponseAsync()
        {
        }

        [Command("wordguardtoggle"), Alias("wgt")]
        [Summary("View WordGuard information.")]
        public async Task ToggleWordGuardResponseAsync()
        {
            await ToggleWordGuardAsync(Context.GetGuildUser(), Context.Guild, Context.Server);
        }

        [Command("crosschat"), Alias("cc")]
        [Summary("View crosschat communication.")]
        public async Task ViewCrossChatResponseAsync()
        {
            await ViewCrossChatAsync(Context.Server);
            Context.Data.Update(Context.Server);
        }

        [Command("crosschattoggle"), Alias("cct")]
        [Summary("Toggle crosschat communication.")]
        public async Task ToggleCrossChatResponseAsync()
        {
            await ToggleCrossChatAsync(Context.GetGuildUser(), Context.Guild, Context.Server);
        }

        [Command("message"), Alias("msg"), Priority(0)]
        [Summary("Send a message to another server.")]
        public async Task CrossChatResponseAsync([Remainder] string content)
        {
            string split = ", ";

            if (!content.Contains(split))
            {
                await ReplyAsync("This sentence was incorrectly parsed! (message GUILD_NAME, MESSAGE)");
                return;
            }

            string[] data = content.Split(split);

            string guild = data[0];
            string message = data[1];

            if (data.Length > 2)
            {
                List<string> msgs = data.ToList();
                msgs.Remove(guild);

                message = string.Join("", msgs);
            }

            if (!Context.TryGetGuild(guild, out SocketGuild g))
            {
                await ReplyAsync("I could not find this guild in this list of guilds I'm in.");
                return;
            }

            await CrossChatAsync(Context.Server, g, message);
        }

        [Command("message"), Alias("msg"), Priority(1)]
        [Summary("Send a message to another server through id.")]
        public async Task CrossChatResponseAsync(ulong id, [Remainder]string message)
        {
            if (!Context.TryGetGuild(id, out SocketGuild g))
            {
                await ReplyAsync("I could not find this guild in this list of guilds I'm in.");
                return;
            }

            await CrossChatAsync(Context.Server, g, message);
        }

        [Command("echo"), Alias("say")]
        [Summary("Sends a message with the following parameters in this channel.")]
        public async Task MessageResponseAsync([Remainder]string content)
        {
            await SendMessageAsync(content);
        }
    }
}