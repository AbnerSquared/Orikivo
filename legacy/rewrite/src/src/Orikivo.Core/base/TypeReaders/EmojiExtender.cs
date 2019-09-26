using Discord;
using System;

namespace Orikivo
{
    public static class EmojiExtender
    {
        public static bool TryParseEmoji(this string s, out Emoji e)
        {
            e = null;
            try
            {
                e = new Emoji(s);
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
    }
}