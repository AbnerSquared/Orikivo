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
        public static readonly long DefaultQuestCapacity = 2;
        public static readonly TimeSpan AssignCooldown = TimeSpan.FromHours(24);
        public static readonly TimeSpan SkipCooldown = TimeSpan.FromHours(4);

        public static bool MeetsCriterion(string questId, string statId, long current)
            => MeetsCriterion(GetQuest(questId), statId, current);

        public static long GetCriterionGoal(string questId, string statId)
            => GetCriterionGoal(GetQuest(questId).Criteria.First(x => x.Id == statId));

        private static string GetDifficultyName(int difficulty)
        {
            if (difficulty >= 5)
                return "Impossible";

            return difficulty switch
            {
                4 => "Extreme",
                3 => "Hard",
                2 => "Normal",
                1 => "Easy",
                _ => "Very Easy"
            };
        }

        private static readonly long DifficultyScalar = 10;
        private static readonly long QuestPointBase = 2;

        private static long GetDefaultValue(int difficulty)
        {
            if (difficulty <= 0)
                return DifficultyScalar;

            return (long)(DifficultyScalar * Math.Pow(QuestPointBase, difficulty - 1));
        }

        public static long GetValue(Quest quest)
            => quest.Value > 0 ? quest.Value : GetDefaultValue(quest.Difficulty);

        public static long GetCriterionGoal(Quest quest, string statId)
            => GetCriterionGoal(quest.Criteria.First(x => x.Id == statId));

        public static bool MeetsCriterion(Quest quest, string criterionId, long current)
        {
            if (quest.Criteria.All(x => x.Id != criterionId))
                return false;

            return current >= GetCriterionGoal(quest.Criteria.First(x => x.Id == criterionId));
        }

        public static bool CanAssign(ArcadeUser user)
        {
            TimeSpan since = StatHelper.SinceLast(user, Stats.Common.LastAssignedQuest);

            return since >= AssignCooldown && user.Quests.Count <= user.GetVar(Stats.Common.QuestCapacity);
        }

        public static bool CanAssign(ArcadeUser user, Quest quest)
        {
            if (!CanAssign(user))
                return false;

            return quest.ToAssign == null || quest.ToAssign(user);
        }

        public static bool Exists(string questId)
            => Assets.Quests.Any(x => x.Id == questId);

        private static bool HasAnyAssignable(ArcadeUser user)
            => Assets.Quests.Any(quest => quest.ToAssign == null || quest.ToAssign(user));

        private static IEnumerable<Quest> GetAssignable(ArcadeUser user)
            => Assets.Quests.Where(quest => quest.ToAssign == null || quest.ToAssign(user));

        public static bool IsAssigned(ArcadeUser user, string questId)
            => Exists(questId) && user.Quests.Any(x => x.Id == questId);

        public static void Assign(ArcadeUser user)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user))
                return;

            if (!HasAnyAssignable(user))
                throw new ArgumentException("The specified user does not have any assignable quests they can use.");

            IEnumerable<Quest> assignable = GetAssignable(user);

            for (int i = 0; i < GetCurrentCapacity(user); i++)
            {
                user.Quests.Add(new QuestData(Randomizer.Choose(assignable)));
            }

            user.SetVar(Stats.Common.LastAssignedQuest, DateTime.UtcNow.Ticks);
        }

        public static Message AssignAndDisplay(ArcadeUser user)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user))
            {
                return new Message($"> 🚫 You have already been assigned your daily objectives.\n> Check back in **{Format.Countdown(StatHelper.GetRemainder(user, Stats.Common.LastAssignedQuest, AssignCooldown))}**.");
            }

            if (!HasAnyAssignable(user))
                return new Message($"> 🚫 You do not meet the criteria to be assigned an objective.");

            IEnumerable<Quest> assignable = GetAssignable(user);

            long available = GetCurrentCapacity(user);

            if (available == 0)
                return new Message($"> 🚫 You don't currently have any room to be assigned any new objectives.");

            var info = new StringBuilder();

            info.AppendLine($"> {Icons.Assign} You have been assigned new objectives!");

            // If you want to allow for preservation of existing quests, ignore ones already specified
            for (int i = 0; i < available; i++)
            {
                Quest toAssign = Randomizer.Choose(assignable);
                user.Quests.Add(new QuestData(toAssign));
                info.AppendLine($"**Slot {i + 1}: {toAssign.Name}** ({GetDifficultyName(toAssign.Difficulty)})");
            }

            TimeSpan amountToSkip = AssignCooldown - ((AssignCooldown / user.GetVar(Stats.Common.QuestCapacity)) * available);

            user.SetVar(Stats.Common.LastAssignedQuest, DateTime.UtcNow.Add(amountToSkip).Ticks);
            user.AddToVar(Stats.Common.TotalAssignedQuests, available);

            return new Message(info.ToString());
        }

        private static long GetCurrentCapacity(ArcadeUser user)
        {
            return Var.GetOrSet(user, Stats.Common.QuestCapacity, DefaultQuestCapacity) - user.Quests.Count;
        }

        public static void Assign(ArcadeUser user, Quest quest)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user, quest))
                return;

            user.Quests.Add(new QuestData(quest));
        }

        public static Quest GetQuest(string questId)
        {
            var quests = Assets.Quests.Where(x => x.Id == questId);

            if (quests.Count() > 1)
                throw new ArgumentException("There is more than 1 quest with the exact same ID.");

            return quests.FirstOrDefault();
        }

        // 0% complete.
        private static string GetProgress(ArcadeUser user, QuestData data)
        {
            Quest quest = GetQuest(data.Id);

            long sum = 0;
            long total = 0;
            foreach (Criterion criterion in quest.Criteria)
            {
                sum += GetCriterionValue(user, data, criterion);
                total += GetCriterionGoal(criterion);
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
                sum.ItemIds.AddOrConcat(reward.ItemIds);
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
                int capacity = (int)Var.GetOrSet(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);
                index = index < 0 ? 0 : index >= capacity ? capacity - 1 : index;
            }

            return user.Quests.ElementAtOrDefault(index);
        }

        public static string TossSlot(ArcadeUser user, int index)
        {
            QuestData slot = GetSlot(user, index);

            if (slot == null)
                return $"> {Icons.Warning} There isn't an assigned objective in this slot.";

            if (user.GetVar(Stats.Common.LastSkippedQuest) > 0)
                if (StatHelper.SinceLast(user, Stats.Common.LastSkippedQuest) >= SkipCooldown)
                    return $"> {Icons.Warning} You have skipped an objective too recently. Try again in {Format.Countdown(SkipCooldown - StatHelper.SinceLast(user, Stats.Common.LastSkippedQuest))}.";

            Quest quest = GetQuest(slot.Id);

            if (quest == null)
                throw new Exception("Expected to find a parent quest but returned null");

            user.Quests.RemoveAt(index);
            user.SetVar(Stats.Common.LastSkippedQuest, DateTime.UtcNow.Ticks);

            TimeSpan since = StatHelper.SinceLast(user, Stats.Common.LastAssignedQuest);

            bool canAssign = since >= AssignCooldown;

            if (!canAssign)
            {
                TimeSpan toSkip = (AssignCooldown - since) / 2; // skip 50% of the remaining time
                user.SetVar(Stats.Common.LastAssignedQuest, new DateTime(user.GetVar(Stats.Common.LastAssignedQuest)).Add(toSkip).Ticks);
            }

            return $"> 🗑️ You have declined the **{quest.Name}** objective.";
        }

        //public static string ViewQuest(string questId)
        //    => ViewQuest(GetQuest(questId));

        public static string ViewSlot(ArcadeUser user, int index)
        {
            int capacity = (int)Var.GetOrSet(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);
            index = index < 0 ? 0 : index >= capacity ? capacity - 1 : index;

            QuestData slot = GetSlot(user, index, false);

            if (slot == null)
                return $"> {Icons.Warning} There isn't an assigned objective in this slot.";

            Quest quest = GetQuest(slot.Id);

            if (quest == null)
                throw new Exception("Expected to find a parent quest but returned null");

            var info = new StringBuilder();

            info.AppendLine($"> **Objective: {quest.Name}** • {GetDifficultyName(quest.Difficulty)} (Slot {index + 1})");
            info.AppendLine($"> {GetProgress(user, slot)}\n");

            info.AppendLine("> **Tasks**");

            foreach (Criterion criterion in quest.Criteria)
            {
                if (!slot.Progress.ContainsKey(criterion.Id))
                {
                    throw new ArgumentException("Expected to find the specified quest criterion to be stored in the progress dictionary");
                }

                string bullet = slot.Progress[criterion.Id].Complete ? "✓" : "•";

                info.AppendLine($"> {bullet} `{criterion.Id}` (**{GetCriterionValue(user, slot, criterion)}** / **{GetCriterionGoal(criterion)}**)");
            }

            info.AppendLine($"\n> **{Format.TryPluralize("Reward", GetUniqueCount(quest.Reward))}**");
            info.AppendLine(WriteReward(quest.Reward));

            return info.ToString();
        }

        private static long GetCriterionValue(ArcadeUser user, QuestData data, Criterion criterion)
        {
            if (!data.Progress.ContainsKey(criterion.Id))
            {
                throw new ArgumentException("Expected to find the specified quest criterion to be stored in the progress dictionary");
            }

            return criterion is VarCriterion ? data.Progress[criterion.Id].Value.GetValueOrDefault(user.GetVar(criterion.Id))
                    : data.Progress[criterion.Id].Complete ? 1 : 0;
        }

        private static long GetCriterionGoal(Criterion criterion)
        {
            return criterion is VarCriterion varCriterion ? varCriterion.ExpectedValue : 1;
        }

        public static bool MeetsCriteria(ArcadeUser user, QuestData data)
        {
            Quest quest = GetQuest(data.Id);

            if (quest == null)
                throw new Exception("Expected to find a parent quest but returned null");

            foreach (Criterion criterion in quest.Criteria)
            {
                if (!data.Progress.ContainsKey(criterion.Id))
                {
                    throw new ArgumentException("Expected to find the specified quest criterion to be stored in the progress dictionary");
                }

                // If the key isn't found, it means that the criterion is not judged by a blank slate, but instead the current value
                long current = criterion is VarCriterion ? data.Progress[criterion.Id].Value.GetValueOrDefault(user.GetVar(criterion.Id))
                    : data.Progress[criterion.Id].Complete ? 1 : 0;

                long goal = criterion is VarCriterion varCriterion ? varCriterion.ExpectedValue : 1;

                if (current < goal)
                    return false;
            }

            return true;
        }

        public static string CompleteAndDisplay(ArcadeUser user)
        {
            if (user.Quests.Count == 0)
                return $"> {Icons.Warning} You do not have any currently assigned objectives.";

            if (!user.Quests.Any(x => MeetsCriteria(user, x)))
                return $"> {Icons.Warning} You have not met the criteria for any currently assigned objectives.";

            List<QuestData> complete = user.Quests.Where(x => MeetsCriteria(user, x)).ToList(); // Creates a new copy

            var info = new StringBuilder();

            int count = complete.Count;

            if (count == 0)
                throw new Exception("Expected at least 1 completed merit but returned 0");

            if (count == 1)
            {
                QuestData completed = complete.FirstOrDefault();
                Quest quest = GetQuest(completed.Id);

                user.Quests.Remove(completed);
                user.AddToVar(Stats.Common.TotalCompletedQuests);
                info.AppendLine($"> **{quest.Name}** ({GetDifficultyName(quest.Difficulty)})");
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
            user.AddToVar(Stats.Common.TotalCompletedQuests, count);
            sum.Apply(user);

            return info.ToString();
        }

        public static string View(ArcadeUser user)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);
            var result = new StringBuilder();

            result.AppendLine("> 🧧 **Objectives**");
            result.AppendLine("> View your currently assigned tasks.");

            int i = 0;
            foreach (QuestData data in user.Quests)
            {
                Quest quest = GetQuest(data.Id);
                result.AppendLine($"\n> **Slot {i + 1}: {quest.Name}** ({GetProgress(user, data)})");

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