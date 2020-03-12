namespace Orikivo.Desync
{
    // TODO: If the ID is unspecified, override the ID
    /// <summary>
    /// Represents a partial <see cref="Construct"/> for a <see cref="Highrise"/>.
    /// </summary>
    public class Floor : Construct
    {
        private string _parentId;

        public override string Id
        {
            get => Check.NotNull(base.Id) ? base.Id : $"{_parentId}#{Level}";
            set => base.Id = value;
        }

        /// <summary>
        /// Represents the level of this <see cref="Floor"/>.
        /// </summary>
        public int Level { get; set; }
    }
}
