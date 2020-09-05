namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a partial <see cref="Construct"/> for a <see cref="Highrise"/>.
    /// </summary>
    public class Floor : Construct
    {
        public Floor()
        {
            Tag = ConstructType.Floor;
            CanUseDecor = true;
        }

        public Floor(int index, string id = null) : this()
        {
            Index = index;
            Id = id;
        }

        internal string ParentId;

        public sealed override string Id
        {
            get => !string.IsNullOrWhiteSpace(base.Id) ? base.Id : $"{ParentId}#{Index}";
            set => base.Id = value;
        }

        /// <summary>
        /// Represents the level of this <see cref="Floor"/>.
        /// </summary>
        public int Index { get; set; }
    }
}
