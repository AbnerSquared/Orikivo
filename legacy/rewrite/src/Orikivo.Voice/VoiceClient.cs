namespace Orikivo.Voice
{
    public interface IOriVoiceClient
    {

    }

    public class OriVoiceClient
    {
        public ulong GuildId {get; set;}
        // the client from which is hosting all audio.
        //public IAudioClient Endpoint {get; set;}
        //public Volume Volume {get; set;}
        public bool Playing {get; set;}
    }
}