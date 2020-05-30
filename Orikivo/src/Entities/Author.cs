using Discord;
using Newtonsoft.Json;

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
        /// Initializes an <see cref="Author"/> with a specified name.
        /// </summary>
        public Author(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new <see cref="Author"/> from a specified <see cref="IUser"/>.
        /// </summary>
        public Author(IUser user) : this(user.ToString())
        {
            Id = user.Id;
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
    }
}
