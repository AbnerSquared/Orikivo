namespace Arcadia.Multiplayer
{
    public class Connection : ReceiverOLD
    {
        public int Frequency { get; set; } // By utilizing C#'s reference system being the same for all referenced values, we can ignore this
        public DisplayContent Content { get; set; }
        public InputController Controller { get; set; }
    }
}
