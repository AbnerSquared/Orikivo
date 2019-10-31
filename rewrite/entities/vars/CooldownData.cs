using System;

namespace Orikivo
{
    public class CooldownData
    {
        public string Id { get; }
        public DateTime ExecutedAt { get; private set; }
        public int Streak { get; private set; }

        public void Refresh(string cooldownId)
        {
            CooldownInfo info = OriSource.GetCooldownInfo(cooldownId);
            double seconds = DateTimeUtils.TimeSince(ExecutedAt).TotalSeconds;
            if (info.CountStreaks)
            {
                if (seconds >= info.Seconds * 2)
                    Streak = 1;
                else
                    Streak++;
            }
            else
                Streak = 0; // ignore streaks if the cooldown doesn't account for it.

            ExecutedAt = DateTime.UtcNow;
        }
    }
}
