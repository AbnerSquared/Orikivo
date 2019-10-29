using Newtonsoft.Json;

namespace Orikivo
{
    /// <summary>
    /// Represents an author, the creator of the object that it is bound to.
    /// </summary>
    public class OriAuthor
    {
        [JsonConstructor]
        internal OriAuthor(string name, ulong? id)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        /// Creates an author with the specified name.
        /// </summary>
        public OriAuthor(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates an author from a specified user.
        /// </summary>
        public OriAuthor(OriUser user) : this(user.ToString())
        {
            Id = user.Id;
        }

        /// <summary>
        /// The name of the author.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// The unique identifier of the author, if one was specified.
        /// </summary>
        [JsonProperty("id")]
        public ulong? Id { get; }
    }
}
