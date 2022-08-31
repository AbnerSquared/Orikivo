using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents the creator of an assigned object.
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Initializes a new <see cref="Author"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="Author"/> to use.</param>
        /// <param name="id">The unique identifier of this <see cref="Author"/>, if the object came from a Discord user.</param>
        [JsonConstructor]
        public Author(string name, ulong? id = null)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        /// Gets a string that represents the name of this <see cref="Author"/>.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// The unique identifier of this <see cref="Author"/>, if one was specified.
        /// </summary>
        [JsonProperty("id")]
        public ulong? Id { get; }

        // TODO: This method seems pointless to specify a fallback string if a default one is already specified?
        /// <summary>
        /// Returns a string that represents this <see cref="Author"/>.
        /// </summary>
        /// <param name="fallback">The fallback name to use if the name is unspecified.</param>
        public string ToString(string fallback)
        {
            return string.IsNullOrWhiteSpace(Name) ? fallback ?? Constants.AUTHOR_UNKNOWN : Name;
        }
    }
}
