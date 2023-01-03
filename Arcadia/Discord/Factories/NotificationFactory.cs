using Arcadia.Services;
using Orikivo;

namespace Arcadia
{
    public static class NotificationFactory
    {
        public static Notification CreateLevelUpdated(int previous, int current, long ascent)
        {
            NotificationType type = NotificationType.LevelUpdated;
            string fmt = "Level up! ({0} to {1})";
            string content = string.Format(fmt, LevelViewer.GetLevel(previous, ascent), LevelViewer.GetLevel(current, ascent));
            return new Notification(content, type);
        }
    }
}
