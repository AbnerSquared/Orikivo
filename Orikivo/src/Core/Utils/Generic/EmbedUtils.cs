using Discord;
using Orikivo.Drawing;
using System;
using System.IO;

namespace Orikivo
{
    public static class EmbedUtils
    {
        /// <summary>
        /// Gets the color object derived from the status of a Discord user.
        /// </summary>
        public static System.Drawing.Color GetColorByStatus(UserStatus status)
        {
            return status.EqualsAny(UserStatus.Offline, UserStatus.Invisible) ? new ImmutableColor(0x747F8D)
                : status == UserStatus.DoNotDisturb ? new ImmutableColor(0xF04747)
                : status.EqualsAny(UserStatus.AFK, UserStatus.Idle) ? new ImmutableColor(0xFAA61A)
                : status == UserStatus.Online ? new ImmutableColor(0x43B581)
                : throw new Exception("The UserStatus given is unspecified.");
        }

        public static string CreateLocalImageUrl(string path)
            => $"attachment://{Path.GetFileName(path)}";
    }
}
