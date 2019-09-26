using Discord;

namespace Orikivo
{
    public static class IActivityExtension
    {
        public static string ToTypeString(this ActivityType a)
            => a.Equals(ActivityType.Listening) ? "Listening to" : $"{a}";

        public static string Summarize(this IActivity a)
            => a.Exists() ? $"{a.Type.ToTypeString()} {a.Name}" : null;

    }
}