using Newtonsoft.Json;
using Orikivo.Unstable;

namespace Orikivo
{
    /// <summary>
    /// Represents the creator of an assigned object.
    /// </summary>
    public class Author
    {
        [JsonConstructor]
        internal Author(string name, ulong? id)
        {
            Name = name;
            Id = id;
        }

        /// <summary>
        /// Constructs an <see cref="Author"/> with a specified name.
        /// </summary>
        public Author(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Constructs an <see cref="Author"/> from a specified <see cref="User"/>.
        /// </summary>
        public Author(User user) : this(user.ToString())
        {
            Id = user.Id;
        }

        /// <summary>
        /// The name of the <see cref="Author"/>.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// The unique identifier of the <see cref="Author"/>, if one was specified.
        /// </summary>
        [JsonProperty("id")]
        public ulong? Id { get; }
    }
}
