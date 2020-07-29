using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orikivo;
using Orikivo.Drawing;

namespace Arcadia
{
    public static class QuestHelper
    {
        public static readonly long DefaultQuestCapacity = 1;
        public static readonly TimeSpan AssignCooldown = TimeSpan.FromHours(24);
        public static readonly List<Quest> Quests = new List<Quest>
        {
            new Quest
            {
                Id = "quest:casino_field_day",
                Name = "Casino Field Day",
                Summary = "It's a wonderful day to gamble your happiness away!",
                Criteria = new List<StatCriterion>
                {
                    new StatCriterion(GimiStats.TimesPlayed, 100),
                    new StatCriterion(TickStats.TimesPlayed, 100)
                },
                Type = QuestType.User
            }
        };

        public static bool CanAssign(ArcadeUser user)
            => StatHelper.SinceLast(user, Stats.LastAssignedQuest) >= AssignCooldown && user.Quests.Count <= user.GetStat(Stats.QuestCapacity);

        public static bool CanAssign(ArcadeUser user, Quest quest)
        {
            if (!CanAssign(user))
                return false;

            return quest.ToAssign == null || quest.ToAssign(user);
        }

        public static bool Exists(string questId)
            => Quests.Any(x => x.Id == questId);

        private static bool HasAnyAssignable(ArcadeUser user)
            => Quests.Any(quest => quest.ToAssign == null || quest.ToAssign(user));

        private static IEnumerable<Quest> GetAssignable(ArcadeUser user)
            => Quests.Where(quest => quest.ToAssign == null || quest.ToAssign(user));

        public static bool IsAssigned(ArcadeUser user, string questId)
            => Exists(questId) && user.Quests.Any(x => x.Id == questId);

        public static void Assign(ArcadeUser user)
        {
            StatHelper.SetIfEmpty(user, Stats.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user))
                return;

            if (!HasAnyAssignable(user))
                throw new ArgumentException("The specified user does not have any assignable quests they can use.");

            IEnumerable<Quest> assignable = GetAssignable(user);

            for (int i = 0; i < GetCurrentCapacity(user); i++)
            {
                user.Quests.Add(new QuestData(Randomizer.Choose(assignable)));
            }

            user.SetStat(Stats.LastAssignedQuest, DateTime.UtcNow.Ticks);
        }

        private static long GetCurrentCapacity(ArcadeUser user)
        {
            return user.GetStat(Stats.QuestCapacity) - user.Quests.Count;
        }

        public static void Assign(ArcadeUser user, Quest quest)
        {
            StatHelper.SetIfEmpty(user, Stats.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user, quest))
                return;

            user.Quests.Add(new QuestData(quest));
        }

        public static Quest GetQuest(string questId)
        {
            var quests = Quests.Where(x => x.Id == questId);

            if (quests.Count() > 1)
                throw new ArgumentException("There is more than 1 quest with the exact same ID.");

            return quests.FirstOrDefault();
        }

        // 0% complete.
        private static string GetProgress(QuestData data)
        {
            Quest quest = GetQuest(data.Id);

            long sum = 0;
            long total = 0;
            foreach (StatCriterion criterion in quest.Criteria)
            {
                sum += data.Progress[criterion.Id];
                total += criterion.ExpectedValue;
            }

            return $"**{MathF.Floor(RangeF.Convert(0, 100, 0, total, sum)):##,0}**% complete";

        }

        public static string ViewCurrent(ArcadeUser user)
        {
            StatHelper.SetIfEmpty(user, Stats.QuestCapacity, DefaultQuestCapacity);
            var result = new StringBuilder();

            result.AppendLine("> 🧧 **Objectives**");
            result.AppendLine("> View your currently assigned tasks.\n");

            int i = 0;
            foreach (QuestData data in user.Quests)
            {
                if (i > 0)
                    result.AppendLine();

                Quest quest = GetQuest(data.Id);
                result.AppendLine($"> **Slot {i}: {quest.Name}** • {quest.Difficulty.ToString()} ({GetProgress(data)})");
                
                if (Check.NotNull(quest.Summary))
                    result.AppendLine($"> {quest.Summary}");

                i++;
            }

            if (i == 0)
            {
                result.AppendLine(Format.Warning("You don't have any assigned objectives!"));
            }

            return result.ToString();
        }
    }
}