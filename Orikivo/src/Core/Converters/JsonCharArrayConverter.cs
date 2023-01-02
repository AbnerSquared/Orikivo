using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Orikivo.Converters.Json
{
    // TODO: Re-create as JsonOptionalArrayConverter<T>
    /// <summary>
    /// A JSON converter used to handle optional <see cref="char"/>[] values. (i.e 'a', ['a', 'b'] => ['a'], ['a', 'b'] )
    /// </summary>
    public class JsonCharArrayConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        public override bool CanConvert(Type objectType)
            => objectType == typeof(char[]) || objectType == typeof(char);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            return token.Type switch
            {
                JTokenType.String => new[] {token.ToObject<char>()},
                JTokenType.Array => token.ToObject<char[]>(),
                _ => new NotSupportedException("The JToken provided did not match a JTokenType of String or Array.")
            };
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="JsonCharArrayConverter"/> can write JSON.
        /// </summary>
        public override bool CanWrite => false;

        /// <summary>
        /// [Not Implemented] Writes the JSON representation of the object.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotSupportedException("This JsonConverter does not support writing.");
    }
}
