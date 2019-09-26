using Newtonsoft.Json;

namespace Orikivo
{
    public class ArrayMap
    {
        public ArrayMap()
        {

        }

        [JsonConstructor]
        public ArrayMap(char[][][] maps)
        {
            Maps = maps;
        }

        [JsonProperty("maps")]
        public char[][][] Maps { get; set; }
    }
}