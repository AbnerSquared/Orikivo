namespace Orikivo
{
    /// <summary>
    /// An update packet specifying how to update a stat.
    /// </summary>
    public class StatUpdatePacket : Int32UpdatePacket
    {
        public StatUpdatePacket(string id, int amount, UpdateMethod method = UpdateMethod.Add) : base(id, amount, method)
        { }
    }
}
