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
        }

        internal string ParentId;

        public override string Id
        {
            get => Check.NotNull(base.Id) ? base.Id : $"{ParentId}#{Index}";
            set => base.Id = value;
        }

        /// <summary>
        /// Represents the level of this <see cref="Floor"/>.
        /// </summary>
        public int Index { get; set; }
    }
}
