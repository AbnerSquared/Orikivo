/*
namespace Orikivo.Voice
{
    // keeps track and stores currently open audio
    // streams.
    public class ChannelCache
    {
        public ConcurrentDictionary<ulong, OriVoiceClient> Connections = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task OpenConnectionAsync(OriGuild g, IVoiceChannel vc)
        {
            if (vc.Guild.Id != g.Id)
                return;

            IAudioClient _acl = await vc.ConnectAsync();
            OriVoiceClient _vcl = new OriVoiceClient(_acl);

            if (Connections.TryGetValue(g.Id, out OriVoiceClient vcl))
                Connections.TryUpdate(g.Id, _vcl, vcl);
            return;
            Connections.TryAdd(g.Id, _vcl);
        }

        public async Task CloseConnectionAsync(OriGuild g)
        {
            if (Connections.TryRemove(g.Id, out OriVoiceClient vcl))
                await vcl.Endpoint.StopAsync();
        }

        public async Task OpenStreamAsync(OriGuild guild)
        {
            if (!Connections.TryGetValue(guild.Id, out OriVoiceClient client))
                return;

            if (guild.Queue.IsEmpty)
                return;

            await WriteStreamAsync(guild, client);

            // use youtube-dl to get audio stream,
            // then ffplay to play the audio, directing its
            // output into the IAudioClient.

            // You get the Output from FFMPEG's handler.
            // Stream is used to read Output.
            
        }

        public async Task WriteStreamAsync(OriGuild guild, OriVoiceClient client)
        {
            // try to find songs first.
            if (guild.Queue.IsEmpty)
                return;

            // definers
            int bufferSize = 1024; // how many buffers it reads each time.
            byte[] bufferRate = new byte[bufferSize]; // a collection of bytes, storing up to the buffer size.
            int offset = 0; // a specified amount of bytes to skip.
            int bitrate = 32000;
            
            Song song = guild.Queue.First; // the first song in the queue.

            Reader:

            // processors
            Stream stdout = OriAudioClient.GetAudioStream(song.Path).StandardOutput.BaseStream;
            AudioOutStream stream = client.Endpoint.CreatePCMStream(AudioApplication.Music, bitrate);
            
            // handlers
            bool open = true;
            long readBytes = 0;

            while (open)
            {
                if (client.Skip)
                {
                    client.Skip = false;
                    open = false;
                    break;
                }

                if (!client.Playing)
                    SpinWait.SpinUntil(() => true != !client.Playing);

                int toWrite = stdout.ReadAsync(bufferRate, offset, bufferSize);
                if (toWrite == 0)
                {
                    open = false;
                    break;
                }

                await stream.WriteAsync(bufferRate, offset, toWrite);
                readBytes += toWrite;
            }

            await stream.FlushAsync();
            //guild.Queue.Remove(song); // this can be a more direct way of removing songs in a queue.
            if (guild.Queue.HasNextSong)
            {
                song = guild.Queue.Next(); // this pushes the first song out of the queue, and returns the next one.
                goto Reader;
            }

            CloseConnectionAsync(guild);
        }
    }
}

*/