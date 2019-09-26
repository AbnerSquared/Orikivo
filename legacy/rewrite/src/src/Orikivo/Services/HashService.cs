using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Orikivo.Systems.Services
{
    public sealed class HashService
    {
        private const int currentVersion = 1;
        private const int saltSize = 16;
        private const int hashSize = 20;
        private const int baseIteration = 1000;

        public static string BuildHash(string password)
        {
            return BuildHash(password, baseIteration);
        }

        public static string BuildHash(string password, int iterationCount)
        {

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);


            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1000);
            byte[] hash = pbkdf2.GetBytes(hashSize);


            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);


            string hashB64 = Convert.ToBase64String(hashBytes);
            string storedHash = string.Format($"_orikiCode?version=1&iteration={iterationCount}&hash={hashB64}");
            return storedHash;
        }

        public static Dictionary<string, string> GetHashInformation(string storedHash)
        {
            var hStart = "_orikiCode?";
            var hVersion = "version=";
            var hIteration = "iteration=";
            var hCore = "hash=";

            var hashCollection = new Dictionary<string, string>();
            if (storedHash.StartsWith(hStart))
            {
                var hashInfo = storedHash.Replace(hStart, string.Empty);
                var hashSet = hashInfo.Split('&');
                var vKeyName = hVersion.Replace("=", string.Empty);
                var iKeyName = hIteration.Replace("=", string.Empty);
                var cKeyName = hCore.Replace("=", string.Empty);

                for (int infoCount = 0; infoCount < hashSet.Length; infoCount++)
                {
                    if (hashSet[infoCount].StartsWith(hVersion) && !hashCollection.ContainsKey(vKeyName))
                    {
                        var version = hashSet[infoCount].Replace(hVersion, string.Empty);
                        hashCollection.Add(vKeyName, version);
                    }
                    else if (hashSet[infoCount].StartsWith(hIteration) && !hashCollection.ContainsKey(iKeyName))
                    {
                        var iteration = hashSet[infoCount].Replace(hIteration, string.Empty);
                        hashCollection.Add(iKeyName, iteration);
                    }
                    else if (hashSet[infoCount].StartsWith(hCore) && !hashCollection.ContainsKey(cKeyName))
                    {
                        var core = hashSet[infoCount].Replace(hCore, string.Empty);
                        hashCollection.Add(cKeyName, core);
                    }
                }

                return hashCollection;
            }
            else
            {
                throw new NotSupportedException("The current hash is not identified as proper.");
            }
        }

        public static bool IsHashOutdated(string storedHash)
        {
            var isValidVersion = int.TryParse(GetHashInformation(storedHash)["version"], out int vId);

            if (isValidVersion)
            {
                return !vId.Equals(currentVersion);
            }
            else
            {
                return true;
            }
        }

        public static bool VerifyHash(string password, string hash)
        {
            if (IsHashOutdated(hash))
            {
                throw new NotSupportedException("The hash type provided is outdated.");
            }

            var hashInfo = GetHashInformation(hash);

            var iteration = int.Parse(hashInfo["iteration"]);
            var hashB64 = hashInfo["hash"];
            var hashBytes = Convert.FromBase64String(hashB64);
            var salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iteration);
            byte[] hashRaw = pbkdf2.GetBytes(hashSize);

            for (var index = 0; index < hashSize; index++)
            {
                if (hashBytes[index + saltSize] != hashRaw[index])
                {
                    return false;
                }
            }
            return true;
            
        }
    }
}
