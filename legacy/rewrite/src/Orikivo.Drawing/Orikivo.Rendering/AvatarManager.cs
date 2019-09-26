using Discord;
using Discord.WebSocket;
using DiscordImageFormat = Discord.ImageFormat;
using System.Drawing;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Orikivo.Static;
using Orikivo.Storage;
using Orikivo.Utility;

namespace Orikivo
{
    public static class AvatarManager
    {
        public static void TrySaveAvatar(SocketUser user, string path, ushort size = 32, DiscordImageFormat format = DiscordImageFormat.Auto)
            => TrySaveAvatar(user.GetAvatarUrl(format, size), path);

        public static void TrySaveAvatar(string url, string path)
        {
            if (!url.Exists() || File.Exists(path))
                return;

            using (WebClient host = new WebClient())
                host.DownloadFile(new Uri(url), path);
        }

        public static void TryEnforceSaveAvatar(string url, string path)
        {
            if (!url.Exists())
                return;

            using (WebClient host = new WebClient())
            {
                host.DownloadFile(new Uri(url), path);
            }
        }

        public static Bitmap GetAvatarBitmap(SocketUser user, ushort size = 32, DiscordImageFormat format = DiscordImageFormat.Auto)
            => GetAvatarBitmap(user.GetAvatarUrl(format, size));

        public static Bitmap GetSetAvatarBitmap(SocketUser user, ushort size = 32, DiscordImageFormat format = DiscordImageFormat.Auto)
            => GetSetAvatarBitmap(user, $"{Directory.CreateDirectory($".//data//{user.Id}//resources//").FullName}avatar{format.GetExtensionName()}", size, format);

        public static Bitmap GetSetAvatarBitmap(SocketUser user, string path, ushort size = 32, DiscordImageFormat format = DiscordImageFormat.Auto, string fallback = null)
            => GetSetAvatarBitmap(user.GetAvatarUrl(format, size), path, fallback);

        public static Bitmap GetSetAvatarBitmap(string url, string path, string fallbackurl = null)
        {
            if (!url.Exists())
            {
                if (fallbackurl.Exists())
                {
                    fallbackurl.Debug();
                    TrySaveAvatar(fallbackurl, path);
                }
                else
                {
                    return new Bitmap(Locator.DefaultAvatar);
                }
            }

            url.Debug();

            TryEnforceSaveAvatar(url, path);
            return new Bitmap(path);
        }

        public static Bitmap GetAvatarBitmap(string url)
        {
            if (!url.Exists())
                return new Bitmap(Locator.DefaultAvatar);

            return (Bitmap)Bitmap.FromStream(GetAvatarMemory(url));
        }

        public static Stream GetAvatarStream(string url)
            => WebRequest.Create(url).GetResponse().GetResponseStream();

        public static byte[] GetRawAvatarData(string url)
            => ToBytes(GetAvatarStream(url));
        
        public static MemoryStream GetAvatarMemory(string url)
            => GetMemory(GetAvatarStream(url));

        public static MemoryStream GetMemory(Stream stream)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory;
            }
        }

        public static byte[] ToBytes(Stream stream)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }

        public static byte[] ToBytes(Stream stream, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            int chunk;

            while((chunk = stream.Read(buffer, bytesRead, (buffer.Length - bytesRead))) > 0)
            {
                bytesRead += chunk;
                if (bytesRead == buffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte == -1)
                        return buffer;
                    
                    byte[] rebuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, rebuffer, buffer.Length);
                    rebuffer[bytesRead] = (byte) nextByte;
                    buffer = rebuffer;
                    bytesRead++;
                }
            }

            byte[] result = new byte[bytesRead];
            Array.Copy(buffer, result, bytesRead);
            return result;
        }
    }
}