namespace Orikivo.Voice
{
    public class ClientVolume
    {
        public static Range Limit = new Range(0, 100);
        public double Value {get {return Muted ? 0d : Value; } set { Value = value; }}
        public bool Muted {get; set;}
        public void Mute()
        {
            //Mute = true;
        }
    }
}