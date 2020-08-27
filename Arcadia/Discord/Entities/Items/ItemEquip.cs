namespace Arcadia
{
    public class ItemEquip
    {
        internal ItemEquip() { }

        // Can this equipment pause the expiration when taken off?
        public bool PauseOnRemove { get; set; }

        // Can this equipment be removed?
        public bool CanRemove { get; set; }
    }
}