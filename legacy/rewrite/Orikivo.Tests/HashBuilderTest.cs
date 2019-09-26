using System;
using System.Collections.Generic;
using System.Text;
using Orikivo.Utility;

namespace Orikivo.Tests
{
    public static class HashBuilderTest
    {
        public static void GetHash()
        {
            Debugger.Write("HashBuilder.Generate...");
            string hash = HashBuilder.Generate("HASH_DEMO");
            hash.Debug();
            Debugger.Write("HashBuilder.Extract...");
            bool extracted = HashBuilder.TryGetHash(hash, out OriHash result);
            if (extracted)
            {
                Debugger.Write("-- Extraction complete. --");
                result.ToString().Debug();
                return;
            }

            Debugger.Write("-- Extraction failed. --");
            return;
        }
    }
}
