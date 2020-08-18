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
                Difficulty = QuestDifficulty.Easy,
                Criteria = new List<StatCriterion>
                {
                    new StatCriterion(GimiStats.TimesPlayed, 100),
                    new StatCriterion(TickStats.TimesPlayed, 100)
                },
                Type = QuestType.User,
                Reward = new Reward
                {
                    Money = 50,
                    Exp = 15
                }
            },
            new Quest
            {
                Id = "quest:trivial_pursuit",
                Name = "Trivial Pursuit",
                Summary = "Test your brain power and push through!",
                Difficulty = QuestDifficulty.Easy,
                Criteria = new List<StatCriterion>
                {
                    new StatCriterion(TriviaStats.TimesPlayed, 5),
                    new StatCriterion(TriviaStats.TimesWon, 1)
                },
                Type = QuestType.User,
                Reward = new Reward
                {
                    Money = 20,
                    Exp = 5
                }
            }
        };

        public static bool MeetsCriterion(string questId, string statId, long current)
            => MeetsCriterion(GetQuest(questId), statId, current);

        public static long GetCriterionGoal(string questId, string statId)
            => GetCriterionGoal(GetQuest(questId), statId);

        public static long GetCriterionGoal(Quest quest, string statId)
        {
            if (quest.Criteria.All(x => x.Id != statId))
                throw new Exception("Expected to find criterion ID but returned null");

            return quest.Criteria.First(x => x.Id == statId).ExpectedValue;
        }

        public static bool MeetsCriterion(Quest quest, string statId, long current)
        {
            if (quest.Criteria.All(x => x.Id != statId))
                return false;

            return current >= quest.Criteria.First(x => x.Id == statId).ExpectedValue;
        }

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

        public static Message AssignAndDisplay(ArcadeUser user)
        {
            StatHelper.SetIfEmpty(user, Stats.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user))
                return new Message($"> 🚫 You have already been assigned your daily objectives.\n> Check back in **{Format.Countdown(StatHelper.GetRemainder(user, Stats.LastAssignedQuest, AssignCooldown))}**.");

            if (!HasAnyAssignable(user))
                return new Message($"> 🚫 You do not meet the criteria to be assigned an objective.");

            IEnumerable<Quest> assignable = GetAssignable(user);

            var info = new StringBuilder();

            info.AppendLine($"> {Icons.Assign} You have been assigned new objectives!");

            // If you want to allow for preservation of existing quests, ignore ones already specified
            for (int i = 0; i < GetCurrentCapacity(user); i++)
            {
                Quest toAssign = Randomizer.Choose(assignable);
                user.Quests.Add(new QuestData(toAssign));
                info.AppendLine($"**Slot {i + 1}: {toAssign.Name}** ({toAssign.Difficulty.ToString()})");
            }

            user.SetStat(Stats.LastAssignedQuest, DateTime.UtcNow.Ticks);

            return new Message(info.ToString());
        }

        private static long GetCurrentCapacity(ArcadeUser user)
        {
            return StatHelper.GetOrAdd(user, Stats.QuestCapacity, DefaultQuestCapacity) - user.Quests.Count;
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

            return $"**{RangeF.Convert(0, total, 0, 100, sum):##,0}**% complete";

        }

        private static int GetUniqueCount(Reward reward)
        {
            int count = reward.ItemIds?.Count ?? 0;

            if (reward.Money > 0)
                count++;

            if (reward.Exp > 0)
                count++;

            return count;
        }

        private static Reward SumRewards(IEnumerable<Reward> rewards)
        {
            var sum = new Reward();

            // Sum up all of the rewards together
            foreach (Reward reward in rewards)
            {
                if (reward == null)
                    continue;

                sum.Money += reward.Money;
                sum.Exp += reward.Exp;
                sum.ItemIds.AddOrSum(reward.ItemIds);
            }

            return sum;
        }

        public static string WriteRewards(IEnumerable<Reward> rewards)
        {
            // Write the finalized reward
            return WriteReward(SumRewards(rewards));
        }

        public static string WriteReward(Reward reward)
        {
            var info = new StringBuilder();

            if (reward.Money > 0)
            {
                info.AppendLine($"> • {Icons.Balance} **{reward.Money:##,0}**");
            }

            if (reward.Exp > 0)
            {
                info.AppendLine($"> • {Icons.Exp} **{reward.Exp:##,0}**");
            }

            if (!Check.NotNullOrEmpty(reward.ItemIds))
                return info.ToString();

            foreach ((string itemId, int amount) in reward.ItemIds)
            {
                string counter = amount > 1 ? $" (x**{amount:##,0}**)" : "";
                info.AppendLine($"> • {ItemHelper.IconOf(itemId)}{ItemHelper.NameOf(itemId)}{counter}");
            }

            return info.ToString();
        }

        private static QuestData GetSlot(ArcadeUser user, int index, bool clamp = true)
        {
            if (clamp)
            {
                int capacity = (int) StatHelper.GetOrAdd(user, Stats.QuestCapacity, DefaultQuestCapacity);
                index = index < 0 ? 0 : index >= capacity ? capacity - 1 : index;
            }

            return user.Quests.ElementAtOrDefault(index);
        }

        public static string TossSlot(ArcadeUser user, int index)
        {
            QuestData slot = GetSlot(user, index);

            if (slot == null)
                return $"> {Icons.Warning} There isn't an assigned objective in this slot.";

            Quest quest = GetQuest(slot.Id);

            if (quest == null)
                throw new Exception("Expected to find a parent quest but returned null");

            user.Quests.RemoveAt(index);
            return $"> 🗑️ You have declined the **{quest.Name}** objective.";
        }

        //public static string ViewQuest(string questId)
        //    => ViewQuest(GetQuest(questId));

        public static string ViewSlot(ArcadeUser user, int index)
        {
            int capacity = (int)StatHelper.GetOrAdd(user, Stats.QuestCapacity, DefaultQuestCapacity);
            index = index < 0 ? 0 : index >= capacity ? capacity - 1 : index;

            QuestData slot = GetSlot(user, index, false);

            if (slot == null)
                return $"> {Icons.Warning} There isn't an assigned objective in this slot.";

            Quest quest = GetQuest(slot.Id);

            if (quest == null)
                throw new Exception("Expected to find a parent quest but returned null");

            var info = new StringBuilder();

            info.AppendLine($"> **Objective: {quest.Name}** (Slot {index + 1})");
            info.AppendLine($"> {GetProgress(slot)}\n");

            info.AppendLine("> **Tasks**");

            foreach (StatCriterion criterion in quest.Criteria)
            {
                string bullet = criterion.ExpectedValue == slot.Progress[criterion.Id] ? "✓" : "•";

                info.AppendLine($"> {bullet} `{criterion.Id}` (**{slot.Progress[criterion.Id]}**/{criterion.ExpectedValue})");
            }

            info.AppendLine($"\n> **{Format.TryPluralize("Reward", GetUniqueCount(quest.Reward))}**");
            info.AppendLine(WriteReward(quest.Reward));

            return info.ToString();
        }

        public static bool MeetsCriteria(QuestData data)
        {
            Quest quest = GetQuest(data.Id);

            if (quest == null)
                throw new Exception("Expected to find a parent quest but returned null");

            foreach (StatCriterion criterion in quest.Criteria)
            {
                long current = data.Progress[criterion.Id];

                if (current < criterion.ExpectedValue)
                    return false;
            }

            return true;
        }


        public static string CompleteAndDisplay(ArcadeUser user)
        {
            if (user.Quests.Count == 0)
                return $"> {Icons.Warning} You do not have any currently assigned objectives.";

            if (!user.Quests.Any(MeetsCriteria))
                return $"> {Icons.Warning} You have not met the criteria for any currently assigned objectives.";

            IEnumerable<QuestData> complete = user.Quests.Where(MeetsCriteria);

            var info = new StringBuilder();

            int count = complete.Count();

            if (count == 0)
                throw new Exception("Expected at least 1 completed merit but returned 0");

            if (count == 1)
            {
                QuestData completed = complete.FirstOrDefault();
                Quest quest = GetQuest(completed.Id);

                user.Quests.Remove(completed);
                info.AppendLine($"> **{quest.Name}** ({quest.Difficulty})");
                info.AppendLine($"> {Icons.Complete} You have completed an objective!\n");
                info.AppendLine("> You have been rewarded:");
                info.AppendLine(WriteReward(quest.Reward));
                quest.Reward.Apply(user);
                return info.ToString();
            }

            user.Quests.RemoveAll(x => complete.Contains(x));
            Reward sum = SumRewards(complete.Select(x => GetQuest(x.Id)?.Reward));
            info.AppendLine($"> {Icons.Complete} You have completed **{count}** objectives!\n");
            info.AppendLine("> You have been rewarded:");
            info.AppendLine(WriteReward(sum));
            sum.Apply(user);

            return info.ToString();
        }

        public static string View(ArcadeUser user)
        {
            StatHelper.SetIfEmpty(user, Stats.QuestCapacity, DefaultQuestCapacity);
            var result = new StringBuilder();

            result.AppendLine("> 🧧 **Objectives**");
            result.AppendLine("> View your currently assigned tasks.");

            int i = 0;
            foreach (QuestData data in user.Quests)
            {
                Quest quest = GetQuest(data.Id);
                result.AppendLine($"\n> **Slot {i + 1}: {quest.Name}** • {quest.Difficulty.ToString()} ({GetProgress(data)})");

                if (Check.NotNull(quest.Summary))
                    result.AppendLine($"> {quest.Summary}");

                i++;
            }

            if (i == 0)
            {
                result.AppendLine($"> {Icons.Warning} You do not have any currently assigned objectives.");
            }

            return result.ToString();
        }
    }
}