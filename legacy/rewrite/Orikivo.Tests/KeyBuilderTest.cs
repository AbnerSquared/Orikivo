using Orikivo.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orikivo.Tests
{
    public static class KeyBuilderTest
    {
        public static OriKeyBatch Regenerate(int iterations, int size = KeyBuilder.DefaultKeyLength)
        {
            Debugger.Write("KeyBuilder.Generate...");
            Debugger.Write($"-- Now generating {iterations.ToPlaceValue()} iteration{(iterations > 1 ? "s" : "")} for a key with the length of {size}. --");
            Task<OriKeyBatch> factory = Task<OriKeyBatch>.Factory.StartNew(() =>
            {
                List<string> keys = new List<string>();

                for(int i = 0; i < iterations; i++)
                {
                    string key = KeyBuilder.Generate(size);
                    if (keys.Contains(key))
                    {
                        Debugger.Write("-- Production failed. --");
                        Debugger.Write($"-- Collision at position {keys.Count}. Key {key} has already been generated at position {(keys.IndexOf(key) + 1).ToPlaceValue()}. --");
                        return new OriKeyBatch(key, keys.Count, keys.IndexOf(key) + 1, keys);
                    }

                    keys.Add(key);
                    continue;
                }

                Debugger.Write($"-- Successful production. No duplicate entries from {iterations.ToPlaceValue()} iteration{(iterations > 1 ? "s": "")}. --");

                return null;
            });

            return factory.Result;
        }
    }
}
