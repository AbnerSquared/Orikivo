namespace Orikivo
{
    public class CardBackground : ItemObject
    {
        // The background of account cards. can be edited accordingly.
        public string Name { get; set; }
        public ulong Cost { get; set; }
        public string Icon { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as ItemObject);
        }

        public bool Equals(ItemObject item)
            => Name == item.Name;

        public override int GetHashCode()
            => unchecked(Name.GetHashCode());
    }
}