using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Orikivo.Systems.Dependencies;
using Orikivo.Systems.Presets;

namespace Orikivo.Modules
{
    [Name("Sound")]
    [Summary("Provides basic access to auditorial interaction.")]
    [DontAutoLoad]
    public class SoundModule : ModuleBase<ICommandContext>
    {
        private readonly CommandService _service;
        private readonly AudioDependency _audio;
        private readonly DiscordSocketClient _socket;
        private readonly IConfigurationRoot _config;
        private readonly CancellationTokenSource _token;

        public SoundModule(CommandService service,
            DiscordSocketClient socket,
            AudioDependency audio,
            IConfigurationRoot config,
            CancellationTokenSource token)
        {
            _service = service;
            _socket = socket;
            _audio = audio;
            _config = config;
            _token = token;
        }

        [Command("kalimba", RunMode = RunMode.Async)]
        [Summary("Play the best song in existance.")]
        public async Task KalimbaAsync()
        {
            var displayEmbed = new EmbedBuilder();
            displayEmbed.WithColor(255, 238, 129);
            displayEmbed.WithTitle("Requested Kalimba.");
            displayEmbed.WithDescription("You are ready to be enrichened to the best song built by mankind...");

            var channel = (Context.User as IGuildUser).VoiceChannel;
            if (channel == null)
            {
                await ReplyAsync("`You gotta join a channel for an ultimate experience like this. :^)`");
                return;
            }
            await _audio.JoinVoiceChannel(Context.Guild, channel);
            var baseMessage = await ReplyAsync(null, false, displayEmbed.Build());

            EmbedBuilder modifiedEmbed = new EmbedBuilder();
            foreach (var embed in baseMessage.Embeds)
            {
                modifiedEmbed = embed.ToEmbedBuilder();
                break;
            }
            if (_audio.IsPlaying(Context.Guild))
            {
                modifiedEmbed.WithColor(129, 243, 193);
                modifiedEmbed.WithTitle("Transcendence failed.");
                modifiedEmbed.WithDescription("Kalimba is too powerful to queue alongside multiple entities.");
            }
            await _audio.BuildStream(Context.Guild, Context.Channel, baseMessage, ".//data//songs//kalimba.mp3");

            modifiedEmbed.WithColor(129, 243, 193);
            modifiedEmbed.WithTitle("Transcendence complete.");
            modifiedEmbed.WithDescription("Kalimba is the best song built by mankind, and nobody can tell me otherwise.");
            await baseMessage.ModifyAsync(x => { x.Embed = modifiedEmbed.Build(); });
            await _audio.LeaveVoiceChannel(Context.Guild);
        }

    [DontAutoLoad]
    [Group("music"), Name("Music"), Alias("m")]
    [Summary("A sub-module that controls all audio related dependencies using the new system.")]
    public class Music : ModuleBase<ICommandContext>
    {
        private readonly CommandService _service;
        private readonly AudioDependency _audio;
        private readonly DiscordSocketClient _socket;
        private readonly IConfigurationRoot _config;
        private readonly CancellationTokenSource _token;
        public Music(CommandService service, DiscordSocketClient socket, AudioDependency audio,
            IConfigurationRoot config, CancellationTokenSource token)
        {
            _service = service;
            _socket = socket;
            _audio = audio;
            _config = config;
            _token = token;
        }

            [Command("connect", RunMode = RunMode.Async), Alias("join", "c")]
            [Summary("Connects to your location or a following voice channel in the guild.")]
            public async Task ConnectAsync([Remainder]IVoiceChannel channel = null)
            {
                channel = channel ?? (Context.User as IGuildUser).VoiceChannel;
                if (channel == null)
                {
                    await ReplyAsync("`I need a channel to reference in order to join what you require.`");
                    return;
                }
                //_audio.OpenStream(Context.Guild);
                await _audio.JoinVoiceChannel(Context.Guild, channel);
            }

            [Command("disconnect", RunMode = RunMode.Async), Alias("leave", "dc")]
            [Summary("Disconnects from the voice channel that it is currently on in the guild.")]
            public async Task DisconnectAsync()
            {
                var queue = _audio.Queue(Context.Guild).ToList();
                var displayEmbed = new EmbedBuilder();
                var displayEmbedFooter = new EmbedFooterBuilder();
                displayEmbed.WithColor(213, 16, 93);
                var selfChannel = Context.Guild.GetCurrentUserAsync().Result.VoiceChannel ?? null;
                if (selfChannel != null)
                {
                    await _audio.LeaveVoiceChannel(Context.Guild);
                    _audio.SetOutput(Context.Guild, false);
                    _audio.CloseStream(Context.Guild);
                    displayEmbed.WithTitle($"I have left {selfChannel.Name}.");
                    await ReplyAsync(null, false, displayEmbed.Build());
                    return;
                }
            }

            [Command("current", RunMode = RunMode.Async), Alias("cur", "np")]
            [Summary("Displays the current audio playing, with a preview of what's next.")]
            public async Task DisplayCurrentAudioAsync()
            {
                var queue = _audio.Queue(Context.Guild).ToList();
                var displayEmbed = new EmbedBuilder();
                var displayEmbedFooter = new EmbedFooterBuilder();
                displayEmbed.WithColor(213, 16, 93);
                displayEmbed.WithTitle("Stopped:");
                if (queue.Count == 0)
                {
                    
                    displayEmbed.WithDescription("Nothing is currently playing.");
                    await ReplyAsync(null, false, displayEmbed.Build());
                    return;
                }
                var prevQueue = queue[0].Value;
                displayEmbed.WithDescription($"{prevQueue.Title} ({prevQueue.Duration})");
                if (_audio.IsPlaying(Context.Guild))
                {
                    displayEmbed.WithColor(129, 243, 193);
                    displayEmbed.WithTitle("Now playing:");
                }
                else if (_audio.Stream(Context.Guild))
                {
                    displayEmbed.WithColor(EmbedData.GetColor("yield"));
                    displayEmbed.WithTitle("Paused:");
                }
                if (queue.Count == 1)
                {
                    await ReplyAsync(null, false, displayEmbed.Build());
                    return;
                }
                var nextQueue = queue[1].Value;
                displayEmbed.WithFooter(displayEmbedFooter.WithText($"Up Next => {nextQueue.Title} ({nextQueue.Duration})"));
                var msg = await ReplyAsync(null, false, displayEmbed.Build());
            }

            [Command("queue", RunMode = RunMode.Async), Alias("q")]
            [Summary("Displays the current list of audio to be played.")]
            public async Task DisplayQueueAsync([Remainder]string getGuild = null)
            {
                IGuild contextGuild = Context.Guild;


                if (getGuild == null)
                {
                    contextGuild = Context.Guild;
                }
                else
                {
                    if (ulong.TryParse(getGuild, out ulong toGuildId))
                    {
                        contextGuild = (await Context.Client.GetGuildsAsync()).FirstOrDefault(x => x.Id == toGuildId);
                    }
                    else
                    {
                        contextGuild = (await Context.Client.GetGuildsAsync()).FirstOrDefault(x => x.Name == getGuild);
                    }
                }
                var queue = _audio.Queue(contextGuild).ToList();
                var currentQueue = queue.ToList();

                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithTitle((contextGuild.Equals(Context.Guild) ? "Current Queue:" : $"Queue [{contextGuild.Name}]"));
                displayEmbed.WithColor(129, 243, 193);

                if (queue.Count == 0)
                {
                    displayEmbed.WithColor(213, 16, 93);
                    displayEmbed.WithDescription("The queue is empty.");
                    await ReplyAsync(null, false, displayEmbed.Build());
                    return;
                }
                if (queue.Count == 1)
                {
                    if (contextGuild == Context.Guild)
                    {
                        displayEmbed.WithColor(213, 16, 93);
                        displayEmbed.WithDescription("The queue is empty.");
                        await ReplyAsync(null, false, displayEmbed.Build());
                        return;
                    }
                }

                if (contextGuild == Context.Guild)
                {
                    currentQueue = queue.Skip(1).ToList();
                }

                var valueString = "";
                foreach (var song in currentQueue)
                {
                    var songInfo = song.Value;
                    valueString += $"[{currentQueue.IndexOf(song)}] {songInfo.Requestant}\n`{songInfo.Title} ({songInfo.Duration})`\n";
                }
                displayEmbed.WithDescription(valueString.TrimEnd('\n'));
                await ReplyAsync(null, false, displayEmbed.Build());
            }

            [Command("raw", RunMode = RunMode.Async), Alias("r")]
            [Summary("Executes a raw process directly to the variables used.")]
            [RequireOwner]
            public async Task ProcessAsync(string fileName, [Remainder]string arguments)
            {
                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithColor(213, 16, 93);
                displayEmbed.WithTitle("Process executed.");
                displayEmbed.WithDescription("Refer to the console for results.");
                var process = _audio.CreateDefaultProcess($"{fileName}", $"{arguments}");
                var processOutput = process.StandardOutput.ReadToEnd();
                if (processOutput.Length > 2000) { processOutput = processOutput.Substring(0, 1992); }
                processOutput = "```\n" + processOutput +"```";

                await ReplyAsync(processOutput, false, displayEmbed.Build());
            }

            //FIX METHOD
            [Command("add", RunMode = RunMode.Async)]
            [Summary("Plays the song specified without downloading.")]
            public async Task PlayAsync([Remainder]string file)
            {
                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithColor(213, 16, 93);
                displayEmbed.WithTitle("Requesting video...");
                displayEmbed.WithDescription("Please wait while the song is searched for.");

                var selfVoice = Context.Guild.GetCurrentUserAsync().Result.VoiceChannel ?? null;
                var contextVoice = (Context.User as IGuildUser).VoiceChannel ?? null;

                if (contextVoice == null && selfVoice == null)
                {
                    await ReplyAsync("`I need a channel to connect to in order to play audio.`");
                    return;
                }
                else if (selfVoice == null)
                {
                    await _audio.JoinVoiceChannel(Context.Guild, contextVoice);
                }

                //await ConnectAsync();
                var audioDisplay = await ReplyAsync(null, false, displayEmbed.Build());
                await _audio.OutputGlobalAudio(Context, Context.Guild, Context.Channel, audioDisplay, file);
            }

            /*[Command("download", RunMode = RunMode.Async), Alias("d")]
            [Summary("Downloads, and then adds the song into the queue.")]
            public async Task DownloadPlayAsync([Remainder]string filePath)
            {
                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithColor(213, 16, 93);
                displayEmbed.WithTitle("Requesting video...");
                displayEmbed.WithDescription("Please wait while the song is searched for.");

                var selfVoice = Context.Guild.GetCurrentUserAsync().Result.VoiceChannel ?? null;
                var contextVoice = (Context.User as IGuildUser).VoiceChannel ?? null;
                
                if (contextVoice == null && selfVoice == null)
                {
                    await ReplyAsync("`I need a channel to connect to in order to play audio.`");
                    return;
                }
                else if (selfVoice == null)
                {
                    await _audio.JoinVoiceChannel(Context.Guild, contextVoice);
                }

                //await ConnectAsync();
                var audioDisplay = await ReplyAsync(null, false, displayEmbed.Build());
                await _audio.OutputLegacyGlobalAudio(Context, Context.Guild, Context.Channel, audioDisplay, filePath);
            }*/

            [Command("continue", RunMode = RunMode.Async), Alias("cont")]
            [Summary("Continues a playlist for a guild, if any exist.")]
            public async Task ContinueQueueAsync()
            {
                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithColor(EmbedData.GetColor("yield"));
                displayEmbed.WithTitle("Continuing queue.");
                displayEmbed.WithDescription("Please wait...");

                var selfVoice = Context.Guild.GetCurrentUserAsync().Result.VoiceChannel ?? null;
                var contextVoice = (Context.User as IGuildUser).VoiceChannel ?? null;

                if (contextVoice == null && selfVoice == null)
                {
                    await ReplyAsync("`I need a channel to connect to in order to play audio.`");
                    return;
                }
                else if (selfVoice == null)
                {
                    await _audio.JoinVoiceChannel(Context.Guild, contextVoice);
                }

                if (_audio.Queue(Context.Guild).Count.Equals(0) || _audio.Stream(Context.Guild)) { return; }

                //await ConnectAsync();
                var display = await ReplyAsync(null, false, displayEmbed.Build());
                await _audio.CheckQueue(Context.Guild, Context.Channel, display);
            }

            [Command("toggle", RunMode = RunMode.Async), Alias("t")]
            [Summary("Toggles play/pause on the currently playing song.")]
            public async Task TogglePlayAsync()
            {
                if (!_audio.Stream(Context.Guild)) return;
                _audio.SetOutput(Context.Guild, !_audio.IsPlaying(Context.Guild));
                var displayEmbed = new EmbedBuilder();
                var yieldColor = EmbedData.GetColor("yield");
                displayEmbed.WithColor(yieldColor);
                displayEmbed.WithTitle("The queue has been paused.");
                if (_audio.IsPlaying(Context.Guild))
                {
                    var oriGreen = EmbedData.GetColor("origreen");
                    displayEmbed.WithColor(oriGreen);
                    displayEmbed.WithTitle("The queue is now playing.");
                }
                await ReplyAsync(null, false, displayEmbed.Build());
            }

            [Command("play", RunMode = RunMode.Async)]
            [Summary("Continues playback on a paused queue.")]
            public async Task SetPlayAsync()
            {
                if (!_audio.Stream(Context.Guild)) return;
                if (_audio.IsPlaying(Context.Guild))
                {
                    return;
                }
                _audio.SetOutput(Context.Guild, true);
                var oriGreen = EmbedData.GetColor("origreen");
                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithColor(oriGreen);
                displayEmbed.WithTitle("The queue is now playing.");
                await ReplyAsync(null, false, displayEmbed.Build());
            }

            [Command("pause", RunMode = RunMode.Async)]
            [Summary("Stops playback on a current queue, until mentioned otherwise.")]
            public async Task SetPauseAsync()
            {
                if (!_audio.Stream(Context.Guild)) return;
                if (!_audio.IsPlaying(Context.Guild))
                {
                    return;
                }
                var yieldColor = EmbedData.GetColor("yield");
                _audio.SetOutput(Context.Guild, false);
                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithColor(yieldColor);
                displayEmbed.WithTitle("The queue has been paused.");
                await ReplyAsync(null, false, displayEmbed.Build());
            }

            [Command("stop", RunMode = RunMode.Async)]
            [Summary("Stops playing the song, and exits the voice channel.")]
            public async Task StopAsync()
            {
                _audio.Stop();
                _audio.CloseStream(Context.Guild);
                _audio.SetOutput(Context.Guild, false);
                await DisconnectAsync();
            }

            [Command("skip", RunMode = RunMode.Async), Alias("s")]
            [Summary("Skips the currently playing song.")]
            [RequireOwner]
            public async Task SetSkipAsync()
            {
                if (_audio.Queue(Context.Guild).Count.Equals(0) || !_audio.Stream(Context.Guild))
                {
                    return;
                }
                var skipColor = EmbedData.GetColor("error");
                if (!_audio.IsPlaying(Context.Guild))
                {
                    _audio.SetOutput(Context.Guild, true);
                    _audio.RequestSkip(Context.Guild);
                }
                else
                {
                    _audio.RequestSkip(Context.Guild);
                }
                var displayEmbed = new EmbedBuilder();
                displayEmbed.WithColor(skipColor);
                displayEmbed.WithTitle("The current song has been skipped.");
                await ReplyAsync(null, false, displayEmbed.Build());
            }

            [Command("information", RunMode = RunMode.Async), Alias("info")]
            [Summary("Pull up all available information of a video from the accepted sources.")]
            public async Task CollectUrlInfoAsync([Remainder]string filePath)
            {
                var infoBase = _audio.CollectAllAudioInformation(filePath);
                var infoEmbed = new EmbedBuilder();
                infoEmbed.WithAuthor(new EmbedAuthorBuilder().WithName(infoBase[0]).WithUrl(infoBase[5]));
                infoEmbed.WithColor(129, 243, 193);
                infoEmbed.WithThumbnailUrl(infoBase[2]);
                infoEmbed.WithDescription($"Identifier: `[{infoBase[1]}]`\nDuration: `[{infoBase[4]}]`");
                infoEmbed.AddField(x => { x.Name = "Description"; x.Value = infoBase[3]; x.IsInline = false; });
                await ReplyAsync(null, false, infoEmbed.Build());
            }
        }
    }
}