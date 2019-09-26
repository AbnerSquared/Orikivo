using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Orikivo.Systems.Services
{
    public class AudioService
    {
        private ConcurrentDictionary<ulong, AudioDataStructure> AudioQuota = new ConcurrentDictionary<ulong, AudioDataStructure>();
        private JsonSerializer _serializer = new JsonSerializer();
        private const string AudioQuotaName = "audioQuota.json";

        public AudioService()
        {
            LoadAudioQuota();
            Clean();
        }

        //Used to reset the output and stream connections on boot.
        public void Clean()
        {
            try
            {
                foreach (var Guild in AudioQuota)
                {
                    AudioDataStructure AudioData = new AudioDataStructure
                    {
                        openStream = false,
                        isPlaying = false,
                        queue = GetQueue(Guild.Key)
                    };
                    AudioDataStructure PreviousAudioData;
                    AudioQuota.TryRemove(Guild.Key, out PreviousAudioData);
                    AudioQuota.AddOrUpdate(Guild.Key, AudioData, ((Key, PreviousValue) => AudioData));
                }
                SaveAudioQuota();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error);
            }
        }

        public AudioDataStructure GetGuildAudioData(IGuild Guild)
        {
            if (AudioQuota.ContainsKey(Guild.Id))
            {
                AudioDataStructure AudioData = new AudioDataStructure();
                AudioQuota.TryGetValue(Guild.Id, out AudioData);
                return AudioData;
            }
            else
            {
                return new AudioDataStructure();
            }
        }

        //Used to retrieve the current skip request.
        public Boolean GetSkipState(IGuild Guild)
        {
            if (AudioQuota.ContainsKey(Guild.Id))
            {
                AudioDataStructure AudioData = new AudioDataStructure();
                AudioQuota.TryGetValue(Guild.Id, out AudioData);
                Boolean SkipState = AudioData.skipRequest;
                return SkipState;
            }
            else
            {
                Console.WriteLine("AudioQuota does not contain this guild's reference.\nSkip state defaulting to false.");
                return false;
            }
        }

        //Used to declare when a user wishes to skip.
        public void SetSkipState(IGuild Guild, Boolean SkipState)
        {
            try
            {
                AudioDataStructure AudioData = new AudioDataStructure
                {
                    skipRequest = SkipState,
                    openStream = GetStreamState(Guild),
                    isPlaying = GetOutputState(Guild),
                    queue = GetQueue(Guild)
                };
                AudioDataStructure PreviousAudioData;
                AudioQuota.TryRemove(Guild.Id, out PreviousAudioData);
                AudioQuota.AddOrUpdate(Guild.Id, AudioData, ((Key, PreviousValue) => AudioData));
                SaveAudioQuota();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error);
            }
        }

        //Used to retrieve the current stream state.
        public Boolean GetStreamState(IGuild Guild)
        {
            if (AudioQuota.ContainsKey(Guild.Id))
            {
                AudioDataStructure AudioData = new AudioDataStructure();
                AudioQuota.TryGetValue(Guild.Id, out AudioData);
                Boolean StreamState = AudioData.openStream;
                return StreamState;
            }
            else
            {
                Console.WriteLine("AudioQuota does not contain this guild's reference.\nStream state defaulting to false.");
                return false;
            }
        }

        //Used to declare if the stream is currently open.
        public void SetStreamState(IGuild Guild, Boolean StreamState)
        {
            try
            {
                AudioDataStructure AudioData = new AudioDataStructure
                {
                    openStream = StreamState,
                    isPlaying = GetOutputState(Guild),
                    queue = GetQueue(Guild)
                };
                AudioDataStructure PreviousAudioData;
                AudioQuota.TryRemove(Guild.Id, out PreviousAudioData);
                AudioQuota.AddOrUpdate(Guild.Id, AudioData, ((Key, PreviousValue) => AudioData));
                SaveAudioQuota();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error);
            }
        }
        //Used to retrieve the current output state.
        public Boolean GetOutputState(IGuild Guild)
        {
            if (AudioQuota.ContainsKey(Guild.Id))
            {
                AudioDataStructure AudioData = new AudioDataStructure();
                AudioQuota.TryGetValue(Guild.Id, out AudioData);
                Boolean StreamState = AudioData.isPlaying;
                return StreamState;
            }
            else
            {
                Console.WriteLine("AudioQuota does not contain this guild's reference.\nOutput state defaulting to false.");
                return false;
            }
        }
        
        //Used to declare if the output is currently playing.
        public void SetOutputState(IGuild Guild, Boolean State)
        {
            try
            {
                AudioDataStructure AudioData = new AudioDataStructure
                {
                    openStream = GetStreamState(Guild),
                    isPlaying = State,
                    queue = GetQueue(Guild)
                };
                AudioDataStructure PreviousAudioData;
                AudioQuota.TryRemove(Guild.Id, out PreviousAudioData);
                AudioQuota.AddOrUpdate(Guild.Id, AudioData, ((Key, PreviousValue) => AudioData));
                SaveAudioQuota();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error);
            }
}

        //Used to get the current queue.
        public Dictionary<String, QueueStructure> GetQueue(IGuild Guild)
        {
            if (AudioQuota.ContainsKey(Guild.Id))
            {
                AudioDataStructure AudioData = new AudioDataStructure();
                AudioQuota.TryGetValue(Guild.Id, out AudioData);
                Dictionary<string, QueueStructure> Queue = AudioData.queue;
                return Queue;
            }
            else
            {
                Console.WriteLine("AudioQuota does not contain this guild's reference.\nQueue defaulting to empty collection.");
                return new Dictionary<string, QueueStructure>();
            }
        }

        public Dictionary<String, QueueStructure> GetQueue(UInt64 Id)
        {
            if (AudioQuota.ContainsKey(Id))
            {
                AudioDataStructure AudioData = new AudioDataStructure();
                AudioQuota.TryGetValue(Id, out AudioData);
                Dictionary<string, QueueStructure> Queue = AudioData.queue;
                return Queue;
            }
            else
            {
                Console.WriteLine("AudioQuota does not contain this guild's reference.\nQueue defaulting to empty collection.");
                return new Dictionary<string, QueueStructure>();
            }
        }

        //Used to add a song to the queue.
        public void AddToQueue(IGuild Guild, String File, QueueStructure SongInformation)
        {
            try
            {
                var Queue = GetQueue(Guild);
                Queue.Add(File, SongInformation);
                AudioDataStructure AudioData = new AudioDataStructure
                {
                    openStream = GetStreamState(Guild),
                    isPlaying = GetOutputState(Guild),
                    queue = Queue
                };
                AudioDataStructure PreviousAudioData;
                AudioQuota.TryRemove(Guild.Id, out PreviousAudioData);
                AudioQuota.AddOrUpdate(Guild.Id, AudioData, ((Key, PreviousValue) => AudioData));
                SaveAudioQuota();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error);
            }
        }

        //Used to remove a song from the queue.
        public void RemoveFromQueue(IGuild Guild, String File)
        {
            try
            {
                var TryQueue = GetQueue(Guild);
                if (TryQueue.ContainsKey(File)) TryQueue.Remove(File); Console.WriteLine("Found and removed specified file.");
                AudioDataStructure AudioData = new AudioDataStructure
                {
                    openStream = true,
                    isPlaying = true,
                    queue = TryQueue
                };
                AudioDataStructure PreviousAudioData;
                AudioQuota.TryRemove(Guild.Id, out PreviousAudioData);
                AudioQuota.AddOrUpdate(Guild.Id, AudioData, ((Key, PreviousValue) => AudioData));
                SaveAudioQuota();
            }
            catch (Exception Error)
            {
                Console.WriteLine(Error);
            }
        }

        private void LoadAudioQuota()
        {
            if (File.Exists(AudioQuotaName))
            {
                using (var JsonText = File.OpenText(AudioQuotaName))
                {
                    using (var Stream = new JsonTextReader(JsonText))
                    {
                        var NewAudioQuota = _serializer.Deserialize<ConcurrentDictionary<ulong, AudioDataStructure>>(Stream);
                        if (NewAudioQuota == null) { return; }
                        AudioQuota = NewAudioQuota;
                    }
                }
            }
            else
            {
                File.Create(AudioQuotaName).Dispose();
            }
        }

        private void SaveAudioQuota()
        {
            if (File.Exists(AudioQuotaName))
            {
                using (var JsonText = File.CreateText(AudioQuotaName))
                {
                    using (var Stream = new JsonTextWriter(JsonText))
                    {
                        Stream.Formatting = Formatting.Indented;
                        _serializer.Serialize(Stream, AudioQuota);
                    }
                }
            }
        }

        public struct AudioDataStructure
        {
            public bool openStream;
            public bool isPlaying;
            public bool skipRequest;
            public Dictionary<String, QueueStructure> queue;
        }

        public struct QueueStructure
        {
            public String Title;
            public String Id;
            public String Duration;
            public String Requestant;
        }
    }
}
