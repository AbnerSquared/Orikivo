namespace Orikivo
{
    /// <summary>
    /// An update packet specifying how to update an upgrade.
    /// </summary>
    public class UpgradeUpdatePacket : Int32UpdatePacket
    {
        public UpgradeUpdatePacket(string id, int amount, UpdateMethod method = UpdateMethod.Add) : base(id, amount, method)
        { }
    }
}
