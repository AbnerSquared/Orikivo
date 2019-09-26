using Discord;
using Discord.Audio;
using Discord.Commands;
using Orikivo.Systems.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orikivo.Systems.Dependencies
{
    public class AudioDependency
    {
        private readonly AudioService data = new AudioService();
        private readonly string songPath = Directory.CreateDirectory(".//Data//Songs//").FullName;
        public ConcurrentDictionary<ulong, IAudioClient> CurrentChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        private CancellationTokenSource token = new CancellationTokenSource();
        public void Stop() => token.Cancel();
        public bool IsPlaying(IGuild guild) => data.GetOutputState(guild);
        public void SetOutput(IGuild guild, bool state) => data.SetOutputState(guild, state);
        public bool Stream(IGuild guild) => data.GetStreamState(guild);
        public void OpenStream(IGuild guild) => data.SetStreamState(guild, true);
        public void CloseStream(IGuild guild) => data.SetStreamState(guild, false);
        public void RequestSkip(IGuild guild) => data.SetSkipState(guild, true);

        public Dictionary<string, AudioService.QueueStructure> Queue(IGuild guild) => data.GetQueue(guild);

        #region VoiceChannels
        public async Task JoinVoiceChannel(IGuild guild, IVoiceChannel destination)
        {
            Console.WriteLine("Now joining a voice channel.");
            if (destination.Guild.Id != guild.Id)
            {
                return;
            }
            var audioClient = await destination.ConnectAsync();
            if (CurrentChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                Console.WriteLine("Audio client found.");
                if (CurrentChannels.TryUpdate(guild.Id, audioClient, client))
                {
                    Console.WriteLine("Updated voice channel location.");
                    Console.WriteLine($"orikivo.cs\nvoice.updated('{guild.Name}')");
                }
                return;
            }
            if (CurrentChannels.TryAdd(guild.Id, audioClient))
            {
                Console.WriteLine("Connected to voice channel.");
                Console.WriteLine($"orikivo.cs\nvoice.connected('{guild.Name}')");
            }
        }

        public async Task LeaveVoiceChannel(IGuild guild)
        {
            Console.WriteLine("Now leaving voice channel.");
            if (CurrentChannels.TryRemove(guild.Id, out IAudioClient client))
            {
                await client.StopAsync();
                Console.WriteLine("Disconnected from voice channel.");
                Console.WriteLine($"orikivo.cs\nvoice.disconnected('{guild.Name}')");
            }
        }
        #endregion

        public async Task CheckQueue(
            IGuild guild,
            IMessageChannel channel,
            IUserMessage display)
        {
            var queue = data.GetQueue(guild);
            if (queue.Count > 0)
            {
                //var songBase = songPath + queue.First().Key;
                var previousEmbed = new EmbedBuilder();
                previousEmbed.WithColor(129, 243, 193);
                previousEmbed.WithTitle($"Now playing:");
                previousEmbed.WithDescription($"{queue.First().Value.Title}");
                await display.ModifyAsync(x => { x.Embed = previousEmbed.Build(); });
                foreach (var embed in display.Embeds) { previousEmbed = embed.ToEmbedBuilder(); break; }
                data.SetOutputState(guild, true);
                await BuildStream(guild, channel, display, queue.First().Key);
                return;
            }
            data.SetOutputState(guild, false);
        }

        public async Task OutputGlobalAudio
            (
                ICommandContext Context,
                IGuild guild,
                IMessageChannel channel,
                IUserMessage display,
                string url
            )
        {
            if (CurrentChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                var path = GetPureAudioUrl(url);
                var queue = data.GetQueue(guild);
                var ffmpeg = CreateFfPlayProcess(path);
                using (var output = ffmpeg.StandardOutput.BaseStream)
                {
                    using (var stream = client.CreatePCMStream(AudioApplication.Music))
                    {
                        var (BufferSize, BufferRate, Offset) = StreamRate().Item1;
                        var closeStream = false;
                        var sentBytes = 0;
                        data.SetStreamState(guild, true);
                        while (!closeStream)
                        {
                            try
                            {
                                //This reads the stream output until there is nothing remaining.
                                var readBytes = await output.ReadAsync(BufferRate, Offset, BufferSize, token.Token);
                                if (readBytes.Equals(0)) { closeStream = true; break; }
                                await stream.WriteAsync(BufferRate, Offset, readBytes, token.Token);
                                sentBytes += readBytes;

                                //A state to determine when the song is paused.
                                while (!data.GetOutputState(guild))
                                {
                                    SpinWait.SpinUntil(() => true != !data.GetOutputState(guild));
                                }

                                //A skip request in use for skipping unwanted songs.
                                if (data.GetSkipState(guild))
                                {
                                    Console.WriteLine("Skip requested.");
                                    data.SetSkipState(guild, false);
                                    closeStream = true;
                                    break;
                                }

                                //A forced exit request in use with stopping ASAP.
                                if (token.IsCancellationRequested)
                                {
                                    Console.WriteLine("Force quit requested.");
                                    data.SetOutputState(guild, false);
                                    data.SetStreamState(guild, false);
                                    await stream.FlushAsync();
                                    closeStream = true;
                                    return;
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
        }

        public async Task OutputLegacyGlobalAudio(
            ICommandContext Context,
            IGuild guild,
            IMessageChannel channel,
            IUserMessage display,
            string url)
        {
            if (CurrentChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                var audioInfo = RetrieveAudioInformation(url);
                if (!File.Exists($"{songPath}{audioInfo.Item4}"))
                {
                    DownloadRequestedAudio(guild, url);
                }

                var previousEmbed = new EmbedBuilder();
                foreach (var embed in display.Embeds) { previousEmbed = embed.ToEmbedBuilder(); break; }
                previousEmbed.WithColor(129, 243, 193);
                previousEmbed.WithTitle($"Song downloaded.");
                previousEmbed.WithDescription($"`{audioInfo.Item1}`");

                var songInfo = new AudioService.QueueStructure
                {
                    Title = audioInfo.Item1,
                    Id = audioInfo.Item2,
                    Duration = audioInfo.Item3,
                    Requestant = Context.User.Username
                };
                data.AddToQueue(guild, audioInfo.Item4, songInfo);

                if (Queue(guild).Count > 0)
                {
                    
                    previousEmbed.WithDescription($"`{audioInfo.Item1}`\nhas been placed in slot {data.GetQueue(guild).Count - 1}.");
                    await display.ModifyAsync(x => { x.Embed = previousEmbed.Build(); });
                    return;
                }

                await display.ModifyAsync(x => { x.Embed = previousEmbed.Build(); });
                await CheckQueue(guild, channel, display);
            }
            else
            {
                await channel.SendMessageAsync("`I need to connect to an audio channel to be able to play music.`");
            }

        }

        public async Task BuildStream(
            IGuild guild,
            IMessageChannel channel,
            IUserMessage display,
            string path)
        {
            var songBase = songPath + path;
            if (!File.Exists(songBase))
            {
                await channel.SendMessageAsync($"`Audio file path mentioned does not exist.`\n`{path}`");
                return;
            }
            if (CurrentChannels.TryGetValue(guild.Id, out IAudioClient client))
            {
                var queue = data.GetQueue(guild);
                var ffmpeg = CreateFfmpegProcess(songBase);
                using (var output = ffmpeg.StandardOutput.BaseStream)
                {
                    using (var stream = client.CreatePCMStream(AudioApplication.Music))
                    {
                        var (BufferSize, BufferRate, Offset) = StreamRate().Item1;
                        var closeStream = false;
                        var sentBytes = 0;
                        data.SetStreamState(guild, true);
                        while (!closeStream)
                        {
                            try
                            {
                                //This reads the stream output until there is nothing remaining.
                                var readBytes = await output.ReadAsync(BufferRate, Offset, BufferSize, token.Token);
                                if (readBytes.Equals(0)) { closeStream = true; break; }
                                await stream.WriteAsync(BufferRate, Offset, readBytes, token.Token);
                                sentBytes += readBytes;

                                //A state to determine when the song is paused.
                                while (!data.GetOutputState(guild))
                                {
                                    SpinWait.SpinUntil(() => true != !data.GetOutputState(guild));
                                }

                                //A skip request in use for skipping unwanted songs.
                                if (data.GetSkipState(guild))
                                {
                                    Console.WriteLine("Skip requested.");
                                    data.SetSkipState(guild, false);
                                    closeStream = true;
                                    break;
                                }

                                //A forced exit request in use with stopping ASAP.
                                if (token.IsCancellationRequested)
                                {
                                    Console.WriteLine("Force quit requested.");
                                    data.SetOutputState(guild, false);
                                    data.SetStreamState(guild, false);
                                    await stream.FlushAsync();
                                    closeStream = true;
                                    return;
                                }
                            }
                            catch (Exception x)
                            {
                                if ((x is TaskCanceledException || x is OperationCanceledException))
                                {
                                    Console.WriteLine("Voice force disconnected.");
                                    data.SetOutputState(guild, false);
                                    data.SetStreamState(guild, false);
                                    await stream.FlushAsync();
                                    closeStream = true;
                                }
                            }
                        }
                        Console.WriteLine("Song ended.");
                        await stream.FlushAsync();
                        data.SetStreamState(guild, false);
                        data.RemoveFromQueue(guild, path);
                        await CheckQueue(guild, channel, display);
                    }
                }
            }
        }

        private void DownloadRequestedAudio(IGuild guild, string url)
        {
            Console.WriteLine("Now downloading requested song...");
            var yDl = CreateDefaultProcess("youtube-dl", $"-v -x --audio-format mp3 -o \"/Data/Songs/ori_%(id)s.%(ext)s\" \"{url}\"");
            yDl.WaitForExit();
            Console.WriteLine($"Downloaded {url}.");
        }

        public Tuple<string, string, string, string> RetrieveAudioInformation(string url)
        {
            Console.WriteLine("Now collecting audio information...");
            string titleBase = "No title is set.";
            string durationBase = ".null";
            string idBase = ".null";
            string localPathReference = "ori_null.mp3";
            var audioInfo = CreateDefaultProcess("youtube-dl", $"--skip-download -s -e --get-id --get-duration \"{url}\"");
            audioInfo.WaitForExit();
            var infoResult = audioInfo.StandardOutput.ReadToEnd();
            Console.WriteLine($"Audio information collected.\n{infoResult}");
            string[] dataCollection = infoResult.Split('\n');
            Console.WriteLine($"Split information results into {dataCollection.Length} segment(s).");
            if (dataCollection.Length >= 2)
            {
                titleBase = dataCollection[0];
                idBase = dataCollection[1];
                durationBase = dataCollection[2];
                localPathReference = $"ori_{idBase}.mp3";
            }
            Console.WriteLine($"Audio information retrieved.\nItem 1 => {titleBase}\nItem 2 => {idBase}\nItem3 => {durationBase}\nItem4 => {localPathReference}");
            Tuple<string, string, string, string> audioInfoSuccess = new Tuple<string, string, string, string>(titleBase, idBase, durationBase, localPathReference);
            Console.WriteLine($"Tuple built. {string.Join("\n", audioInfoSuccess)}");
            return audioInfoSuccess;
        }

        public string GetPureAudioUrl(string url)
        {
            var yDl = CreateDefaultProcess("youtube-dl", $"-f bestaudio -g \"{url}\"");
            yDl.WaitForExit();
            var rawUrl = yDl.StandardOutput.ReadToEnd();
            return rawUrl;
        }

        public List<string> CollectAllAudioInformation(string url)
        {
            Console.WriteLine("Now collecting all available audio information...");
            string globalPathReference = "https://www.youtube.com/embed/";
            string publicPathReference = "https://www.youtube.com/watch?v=";
            var yDl = CreateDefaultProcess("youtube-dl", $"-v -s" +
                $" --get-title --get-id --get-thumbnail --get-duration" +
                $" {url}");
            var yDlDescription = CreateDefaultProcess("youtube-dl", $"-v -s --get-description \"{url}\"");

            var jsonInfo = yDl.StandardOutput.ReadToEnd();
            var descInfo = yDlDescription.StandardOutput.ReadToEnd();
            var splitInfo = jsonInfo.Split("\n");

            Console.WriteLine($"Collected all available information.\n>>{string.Join("\n>>", splitInfo)}");
            globalPathReference += $"{splitInfo[1]}?autoplay=true";
            publicPathReference += splitInfo[1];
            var fieldSpace = descInfo.Length;
            if (fieldSpace > 1024)
            {
                fieldSpace = 1023;
            }
            var organizedInformation = new List<string> { splitInfo[0], splitInfo[1], splitInfo[2], descInfo.Substring(0, fieldSpace), splitInfo[3], globalPathReference, publicPathReference };
            return organizedInformation;
        }

        public Tuple<(int BufferSize, byte[] BufferRate, int Offset)> StreamRate()
        {
            var baseSize = 1024;
            return new Tuple<(int BufferSize, byte[] BufferRate, int Offset)>((baseSize, new byte[baseSize], 0));
        }

        #region Processes
        public Process CreateDefaultProcess(string fileType, string arguments)
        {
            Console.WriteLine($"Now building a new process using {fileType}, with the args of '{arguments}'.");
            return Process.Start(new ProcessStartInfo
            {
                FileName = fileType,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        private Process CreateFfmpegProcess(string path)
        {
            Console.WriteLine("Now starting FFMPEG...");
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        private Process CreateFfPlayProcess(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffplay",
                Arguments = $"-i \"{path}\" -ac 2 -ar 48000",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        private Process CreateDirectFfmpegProcess(string stream)
        {
            Console.WriteLine("Now starting FFMPEG...");
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{stream}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }
        #endregion

    }
}
