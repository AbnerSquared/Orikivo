using System.Text;

namespace Arcadia.Services
{
    public class LevelViewer
    {
        public static string View(ArcadeUser user, bool showName = false)
        {
            var details = new StringBuilder();

            int level = user.Level;

            if (showName)
            {
                details.AppendLine($"**{user.Username}**");
            }

            details.AppendLine($"> {GetLevel(level, user.Ascent)}");
            details.Append($"> {Icons.Exp} **{user.Exp:##,0} XP**");

            if (level < ExpConvert.MaxLevel)
            {
                details.Append($" (**{ExpConvert.ExpToNext(user.Exp, user.Ascent)} XP** to {GetLevel(level + 1, user.Ascent)})");
            }

            return details.ToString();
        }

        public static string GetLevel(int level, int ascent)
        {
            return $"v**{ascent}.{level:00}**";
        }
    }
}