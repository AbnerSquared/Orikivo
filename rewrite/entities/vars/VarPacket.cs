namespace Orikivo
{
    // variable update packet for a generic var.
    public class VarPacket
    {
        public string VarId { get; }
        public object Value { get; } // ensure value type matches varid type
    }
}
