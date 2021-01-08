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
        public static readonly TimeSpan AssignCooldown = TimeSpan.FromHours(6);
        public static readonly TimeSpan SkipCooldown = TimeSpan.FromHours(2);
        private static readonly long DifficultyScalar = 10;
        private static readonly long QuestPointBase = 2;
        private static readonly long BaseMoneyReward = 20;
        private static readonly long MoneyDifficultyScale = 10;
        private static readonly long BaseExpReward = 15;
        private static readonly long ExpDifficultyScale = 5;

        public static string View(ArcadeUser user)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);
            var result = new StringBuilder();

            result.AppendLine($"> {Icons.Quests} **Quests**");
            result.AppendLine($"> Quest Points Earned: **{user.GetVar(Vars.QuestPoints):##,0}** (**{user.GetVar(Vars.MonthlyQuests):##,0}** this month)");

            int i = 0;
            foreach (QuestData data in user.Quests)
            {
                Quest quest = GetQuest(data.Id);
                result.AppendLine($"> **Slot {i + 1}: {quest.Name}** ({GetQuestCompletion(user, data)})");

                //if (Check.NotNull(quest.Summary))
                //    result.AppendLine($"> {quest.Summary}");

                i++;
            }

            if (i == 0)
            {
                result.AppendLine(Format.Warning("You do not have any quests assigned to you. Type `assign` to get started!"));
            }

            //result.AppendLine($"> {Icons.Challenges} **Challenges**");
            //result.AppendLine($"> Complete your daily quests to be given challenges.");

            return result.ToString();
        }

        public static string InspectQuest(ArcadeUser user, int index)
        {
            int capacity = (int)Var.GetOrSet(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);
            index = index < 0 ? 0 : index >= capacity ? capacity - 1 : index;

            QuestData slot = GetQuestAt(user, index, false);

            if (slot == null)
                return Format.Warning("There isn't a quest at that slot.\n\n") + View(user);

            Quest quest = GetQuest(slot.Id);
            Reward reward = quest.Reward ?? GetDefaultReward(quest.Difficulty);

            if (quest == null)
                throw new Exception("Expected to find a parent quest but returned null");

            var info = new StringBuilder();

            info.AppendLine($"> **{quest.Name}**");

            if (Check.NotNull(quest.Summary))
                info.AppendLine($"> {quest.Summary}\n");

            info.AppendLine($"> {GetDifficultyName(quest.Difficulty)} (Slot {index + 1}) • {GetQuestCompletion(user, slot)}\n");

            info.AppendLine("> **Tasks**");

            foreach (Criterion criterion in quest.Criteria)
            {
                if (!slot.Progress.ContainsKey(criterion.Id))
                {
                    throw new ArgumentException("Expected to find the specified quest criterion to be stored in the progress dictionary");
                }

                string bullet = slot.Progress[criterion.Id].Complete ? "✓" : "•";
                string name = Check.NotNull(criterion.Name) ? criterion.Name : $"`{criterion.Id}`";

                info.AppendLine($"> {bullet} {name} (**{GetCriterionValue(user, slot, criterion)}** / **{GetCriterionGoal(criterion)}**)");
            }

            info.AppendLine($"\n> **Completion {Format.TryPluralize("Reward", reward.Count)}**");
            info.AppendLine(reward.ToString());

            return info.ToString();
        }

        public static Message AssignAndDisplay(ArcadeUser user)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user))
                return new Message(Format.Warning("You are asking for quests too quickly!") + $"\n> Check back in **{Format.Countdown(StatHelper.GetRemainder(user, Stats.Common.LastAssignedQuest, AssignCooldown))}**.");

            if (!HasAnyAssignable(user))
                return new Message(Format.Warning("There isn't any available quests for you at the moment. Come back later."));

            IEnumerable<Quest> assignable = GetAssignable(user);

            long available = GetCurrentCapacity(user);

            if (available == 0)
                return new Message(Format.Warning("You don't have any room to be assigned new quests!"));

            var info = new StringBuilder();

            info.AppendLine($"> {Icons.Assign} You have been assigned {(available == 1 ? "a new quest" : $"**{available}** quests")}!");

            // If you want to allow for preservation of existing quests, ignore ones already specified
            for (int i = 0; i < available; i++)
            {
                Quest toAssign = Randomizer.Choose(assignable);
                user.Quests.Add(new QuestData(toAssign));
                info.AppendLine($"**Slot {i + 1}: {toAssign.Name}** ({GetDifficultyName(toAssign.Difficulty)})");
            }

            TimeSpan amountToSkip = GetSkipDuration(user.GetVar(Stats.Common.QuestCapacity), available);

            user.SetVar(Stats.Common.LastAssignedQuest, DateTime.UtcNow.Add(amountToSkip).Ticks);
            user.AddToVar(Stats.Common.TotalAssignedQuests, available);

            return new Message(info.ToString());
        }

        public static string CompleteAndDisplay(ArcadeUser user)
        {
            if (user.Quests.Count == 0)
                return Format.Warning("You do not have any quests assigned to you. Type `assign` to get started!");

            if (!user.Quests.Any(x => MeetsCriteria(user, x)))
                return Format.Warning("You have not met the criteria for any of your quests.");

            List<QuestData> completed = user.Quests.Where(x => MeetsCriteria(user, x)).ToList(); // Creates a new copy

            var info = new StringBuilder();

            if (completed.Count == 0)
                throw new Exception("Expected to find a completed quest but returned empty");

            user.Quests.RemoveAll(x => completed.Contains(x));
            long points = completed.Sum(x => GetWorth(GetQuest(x.Id)));
            Reward sum = SumRewards(completed.Select(x => GetQuest(x.Id).Reward ?? GetDefaultReward(GetQuest(x.Id).Difficulty)));

            info.AppendLine(GetCompletionHeader(ref completed))
                .AppendLine("> Here is your reward:")
                .AppendLine(sum.ToString());

            user.AddToVar(Stats.Common.TotalCompletedQuests, completed.Count);
            Var.Add(user, points, Vars.QuestPoints, Vars.MonthlyQuests);
            sum.Apply(user);

            return info.ToString();
        }

        public static string SkipAndDisplay(ArcadeUser user, int index)
        {
            QuestData slot = GetQuestAt(user, index);

            if (slot == null)
                return Format.Warning("There isn't a quest at that slot.\n\n") + View(user);

            if (user.GetVar(Stats.Common.LastSkippedQuest) > 0 && StatHelper.SinceLast(user, Stats.Common.LastSkippedQuest) >= SkipCooldown)
                return $"> {Icons.Warning} You already skipped a quest too recently. Try again in {Format.Countdown(SkipCooldown - StatHelper.SinceLast(user, Stats.Common.LastSkippedQuest))}.";

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

                return $"> {Icons.Skip} You have skipped **{quest.Name}** and reduced your assign cooldown by {Format.Counter(toSkip)}.";
            }

            return $"> {Icons.Skip} You have skipped **{quest.Name}**.";
        }

        public static Quest GetQuest(string questId)
        {
            var quests = Assets.Quests.Where(x => x.Id == questId);

            if (quests.Count() > 1)
                throw new ArgumentException("There is more than 1 quest with the exact same ID.");

            return quests.FirstOrDefault();
        }

        public static void Assign(ArcadeUser user, Quest quest)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user, quest))
                return;

            user.Quests.Add(new QuestData(quest));
        }

        public static bool MeetsCriterion(string questId, string statId, long current)
            => MeetsCriterion(GetQuest(questId), statId, current);

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

        public static long GetCriterionGoal(string questId, string statId)
            => GetCriterionGoal(GetQuest(questId).Criteria.First(x => x.Id == statId));

        public static long GetWorth(Quest quest)
            => quest.Value > 0 ? quest.Value : GetDefaultWorth(quest.Difficulty);

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

        public static bool IsAssigned(ArcadeUser user, string questId)
            => Exists(questId) && user.Quests.Any(x => x.Id == questId);

        public static void Assign(ArcadeUser user)
        {
            Var.SetIfEmpty(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);

            if (!CanAssign(user))
                return;

            if (!HasAnyAssignable(user))
                throw new ArgumentException("The specified user does not have any assignable quests.");

            IEnumerable<Quest> assignable = GetAssignable(user);

            for (int i = 0; i < GetCurrentCapacity(user); i++)
            {
                user.Quests.Add(new QuestData(Randomizer.Choose(assignable)));
            }

            user.SetVar(Stats.Common.LastAssignedQuest, DateTime.UtcNow.Ticks);
        }

        private static TimeSpan GetSkipDuration(long questCapacity, long available)
        {
            return AssignCooldown - ((AssignCooldown / questCapacity) * available);
        }

        private static long GetCurrentCapacity(ArcadeUser user)
        {
            return Var.GetOrSet(user, Stats.Common.QuestCapacity, DefaultQuestCapacity) - user.Quests.Count;
        }

        private static string GetQuestCompletion(ArcadeUser user, QuestData data)
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

        private static Reward SumRewards(IEnumerable<Reward> rewards)
        {
            var sum = new Reward();

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

        private static QuestData GetQuestAt(ArcadeUser user, int index, bool clamp = true)
        {
            if (clamp)
            {
                int capacity = (int)Var.GetOrSet(user, Stats.Common.QuestCapacity, DefaultQuestCapacity);
                index = index < 0 ? 0 : index >= capacity ? capacity - 1 : index;
            }

            return user.Quests.ElementAtOrDefault(index);
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

        private static string GetCompletionHeader(ref List<QuestData> completed)
        {
            if (completed.Count == 1)
            {
                QuestData data = completed.First();
                Quest quest = GetQuest(data.Id);

                return $"> {Format.Bold(quest.Name)} ({GetDifficultyName(quest.Difficulty)})\n> {Icons.Complete} You have fulfilled this quest!\n";
            }

            return $"> {Icons.Complete} You have fulfilled **{completed.Count:##,0}** quests!\n";
        }

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

        private static long GetBaseMoney(int difficulty)
        {
            return BaseMoneyReward + (MoneyDifficultyScale * difficulty);
        }

        private static ulong GetBaseExp(int difficulty)
        {
            return (ulong)(BaseExpReward + (ExpDifficultyScale * difficulty));
        }

        private static Reward GetDefaultReward(int difficulty)
        {
            return new Reward
            {
                Money = GetBaseMoney(difficulty),
                Exp = GetBaseExp(difficulty)
            };
        }

        private static long GetDefaultWorth(int difficulty)
        {
            if (difficulty <= 0)
                return DifficultyScalar;

            return (long)(DifficultyScalar * Math.Pow(QuestPointBase, difficulty - 1));
        }

        private static bool HasAnyAssignable(ArcadeUser user)
            => Assets.Quests.Any(quest => quest.ToAssign == null || quest.ToAssign(user));

        private static IEnumerable<Quest> GetAssignable(ArcadeUser user)
            => Assets.Quests.Where(quest => quest.ToAssign == null || quest.ToAssign(user));
    }
}