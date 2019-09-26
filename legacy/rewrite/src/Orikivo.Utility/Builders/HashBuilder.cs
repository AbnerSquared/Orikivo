using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Orikivo.Utility
{
    public sealed class HashBuilder
    {
        public const int Version = 1;
        private const int _saltSize = 16;
        private const int _hashSize = 20;
        private const int _hashByteSize = 36;
        private const int _defaultIteration = 1000;
        public const string HashFormat = "_oriHash?version={0}&iteration={1}&hash={2}";
        public const string RegexPattern = @"_oriHash\?version=(\d)&iteration=(\d+)&hash=([a-zA-Z0-9/\+]+$)";

        public static string Generate(string value)
            => Generate(value, _defaultIteration);

        public static string Generate(string value, int iterations)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[_saltSize]);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(value, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(_hashSize);
            byte[] hashBytes = new byte[_hashByteSize];
            Array.Copy(salt, 0, hashBytes, 0, _saltSize);
            Array.Copy(hash, 0, hashBytes, _saltSize, _hashSize);

            string hashBase64 = Convert.ToBase64String(hashBytes);
            return string.Format(HashFormat, Version, iterations, hashBase64);
        }

        public static bool TryGetHash(string hash, out OriHash result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(hash))
                return false;

            Regex regex = new Regex(RegexPattern);
            RegexPattern.Debug();
            if (regex.IsMatch(hash))
            {
                Debugger.Write("-- The hash matched the pattern. --");
                Match match = regex.Match(hash);
                match.Groups.ForEach(x => x.Value.Debug($"Index {x.Index}; Length {x.Length}"));

                string[] data = new string[3];
                match.Groups.Enumerate(x => x.Value).Skip(1).ToArray().CopyTo(data, 0); // skip one since the first one is the entire capture
                result = new OriHash(int.Parse(data[0]), int.Parse(data[1]), data[2]);
                return true;
            }

            return false;
        }
    }
}
