using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // Used to convert a JSON file with optional arrays to such.
    public class DynamicArrayJsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(char[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.String)
                return new char[] { token.ToObject<char>() };

            if (token.Type == JTokenType.Array)
                return token.ToObject<char[]>();

            return new char[] { token.ToObject<char>() };
        }

        public override bool CanWrite { get { return false; } }

        // find out a way to serialize this json format
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
