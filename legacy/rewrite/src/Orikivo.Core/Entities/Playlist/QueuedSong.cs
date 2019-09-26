namespace Orikivo
{
    public class QueuedSong
    {
        public ulong QueuerId {get; private set;}
        public string Id {get; set;}
        public string Url {get; private set;}
        public SongData Data {get; private set;}
    }
}