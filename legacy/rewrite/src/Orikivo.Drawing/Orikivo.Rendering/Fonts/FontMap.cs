using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{

    public class DynamicArrayJsonConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(char[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            token.Type.Debug("Token Type");
            
            if (token.Type == JTokenType.String)
            {
                return new char[] { token.ToObject<char>() };
            }

            if (token.Type == JTokenType.Array)
                return token.ToObject<char[]>();
            
            return new char[] { token.ToObject<char>() };
        }

        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Represents the collection of fonts that are referenced upon launch.
    /// </summary>
    public class FontCache
    {
        public FontCache(List<FontFace> fonts, char[][][][] arrayMap)
        {
            Fonts = fonts;
            ArrayMap = arrayMap;
        }

        public List<FontFace> Fonts { get; set; } // a list of font faces.

        public char[][][][] ArrayMap { get; set; } // defines where a letter is rendered on a font sheet.

        public FontFace GetFont(ulong id)
        {
            try
            {
                return Fonts.Where(x => x.Id == id).First();
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        public (int i, int x, int y) GetMapIndex(char c)
        {
            (int i, int x, int y) pos = new ValueTuple<int, int, int>();
            foreach (char[][][] map in ArrayMap)
            {
                if (map.Any(x => x.Any(y => y.Contains(c))))
                {
                    pos.i = ArrayMap.ToList().IndexOf(map);
                    foreach (char[][] row in map)
                    {
                        string.Join("", row.Enumerate(x => string.Join("", x)).Conjoin("")).Debug();
                        if (row.Any(x => x.Contains(c)))
                        {
                            pos.y = map.ToList().IndexOf(row);
                            foreach (char[] item in row)
                            {
                                if (item.Contains(c))
                                {
                                    pos.x = row.ToList().IndexOf(item);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            return pos;
        }
    }
}