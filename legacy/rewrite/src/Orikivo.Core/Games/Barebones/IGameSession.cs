namespace Orikivo
{
    public interface IGameSession
    {
        ulong ChannelId {get; set;}
        ulong MessageId {get; set;}
        string GameId {get; set;}
        //IGameData Data {get; set;}
    }
}