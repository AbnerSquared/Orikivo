namespace Orikivo.Utility
{
    public class OriHash
    {
        public OriHash(int version, int iterations, string hash)
        {
            Version = version;
            IterationCount = iterations;
            Hash = hash;
        }

        public int Version { get; }
        public int IterationCount { get; }
        public string Hash { get; }

        public override string ToString()
            => string.Format(HashBuilder.HashFormat, Version, IterationCount, Hash);
    }
}
