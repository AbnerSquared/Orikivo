using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arcadia.Multiplayer;
using Orikivo;

namespace Arcadia
{
    public static class ChallengeHelper
    {
        public static readonly int SetSize = 3;

        public static IEnumerable<Challenge> GetChallengeSet(long completedSets)
        {
            // Range of difficulty increases per completed set
            // 0 => 0-1
            // n => 0 to 1+n

            long minDifficulty = 0; // This will be changed later to increase up to the highest known bound
            long maxDifficulty = 1 + completedSets;
            var challenges = Assets.Challenges.Where(x => x.Difficulty >= minDifficulty && x.Difficulty <= maxDifficulty); // Assets.Quests.Where(x => x.Type == QuestType.Challenge && x.Difficulty >= minDifficulty && x.Difficulty <= maxDifficulty); // Assets.Challenges.Where(x => x.Difficulty >= minDifficulty && x.Difficulty <= maxDifficulty);

            return Randomizer.ChooseMany(challenges, SetSize);
        }

        private static readonly int BaseMoney = 20;
        private static readonly uint BaseExp = 25;
        private static readonly int MoneyScalar = 5;
        private static readonly uint ExpScalar = 5;
        private static readonly long ChallengeCap = 5;

        private static Reward GetReward(long dailyCompleted)
        {
            float baseScale = 1.0f;

            if (dailyCompleted > ChallengeCap)
                baseScale = 0.6f;

            var reward = new Reward
            {
                Money = (long)MathF.Floor((BaseMoney + (dailyCompleted * MoneyScalar)) * baseScale),
                Exp = (long)MathF.Floor((BaseExp + (dailyCompleted * ExpScalar)) * baseScale)
            };

            // Set the reward scalar based on completed sets
            // Base money: 20 + 5 * completed sets
            // Base experience: 25 + 5 * completed sets

            return reward;
        }

        public static string View(ArcadeUser user)
        {
            if (user.Challenges.Count == 0)
            {
                Assign(user);
            }

            var result = new StringBuilder();

            long completed = user.GetVar(Stats.Challenge.SetsCompletedDaily);

            result.AppendLine("> **Challenges**");
            result.AppendLine($"> Sets Completed Today: **{completed}**");
            result.AppendLine();

            result.AppendLine($"> **Set {completed + 1}**");

            foreach(Challenge challenge in user.Challenges.Keys.Select(GetChallenge))
            {
                result.AppendLine($"{(user.Challenges[challenge.Id].Complete ? "✓" : "•")} {challenge.Name}");
            }

            result.AppendLine();
            result.AppendLine("> **Reward**");
            Reward reward = GetReward(completed);

            if (reward.Money > 0)
                result.AppendLine($"> {CurrencyHelper.WriteCost(reward.Money, CurrencyType.Cash)}");

            if (reward.Exp > 0)
                result.AppendLine($"> {Icons.Exp} **{reward.Exp:##,0}**");

            if (Check.NotNullOrEmpty(reward.ItemIds))
            {
                foreach ((string itemId, int amount) in reward.ItemIds)
                    result.AppendLine($"> {GetItemPreview(itemId, amount)}");
            }

            return result.ToString();
        }

        private static string GetItemPreview(string itemId, int amount)
        {
            string icon = ItemHelper.GetIconOrDefault(itemId) ?? "•";
            string name = Check.NotNull(icon) ? ItemHelper.GetBaseName(itemId) : ItemHelper.NameOf(icon);
            string counter = amount > 1 ? $" (x**{amount:##,0}**)" : "";
            return $"`{itemId}` {icon} **{name}**{counter}";
        }

        public static void Assign(ArcadeUser user)
        {
            if (user.Challenges.Count == 0)
            {
                foreach (Challenge challenge in GetChallengeSet(user.GetVar(Stats.Challenge.SetsCompletedDaily)))
                {
                    user.Challenges.Add(challenge.Id, new ChallengeData(challenge));
                }
            }
        }

        public static string Submit(ArcadeUser user)
        {
            if (!CanSubmit(user))
                return Format.Warning("You have not met the criteria for all of the challenges.");

            GetReward(user.GetVar(Stats.Challenge.SetsCompletedDaily)).Apply(user);
            user.AddToVar(Stats.Challenge.SetsCompletedDaily);
            user.Challenges.Clear();

            foreach (Challenge challenge in GetChallengeSet(user.GetVar(Stats.Challenge.SetsCompletedDaily)))
            {
                user.Challenges.Add(challenge.Id, new ChallengeData(challenge));
            }

            return "> You have completed a challenge set! A new one has been assigned in its place.";
        }

        public static bool CanSubmit(ArcadeUser user)
        {
            return user.Challenges.All(x => x.Value.Complete);
        }

        public static Challenge GetChallenge(string id)
        {
            return Assets.Challenges.FirstOrDefault(x => x.Id == id);
        }

        // New challenges cannot be given until the entire set is completed.

        private static IEnumerable<Quest> GetActiveQuests(ArcadeUser user)
        {
            return user.Quests
                .Where(x => !QuestHelper.MeetsCriteria(user, x))
                .Select(x => QuestHelper.GetQuest(x.Id));
        }

        public static void UpdateChallengeProgress(ArcadeUser user, GameResult result)
        {
            if (user.Quests.Count == 0)
                return;

            var ctx = new CriterionContext(user, result);

            // Get the first matching quest
            // TODO: Get the best matching quest
            Quest target = GetActiveQuests(user).Where(x => x.Criteria.Any(y => y.Triggers == CriterionTriggers.Game)).FirstOrDefault();

            if (target == null)
                return;

            foreach (Criterion criterion in target.Criteria)
            {
                if (criterion.Judge(ctx))
                {
                    user.Quests.First(x => x.Id == target.Id).Progress[criterion.Id].Complete = true;
                }
            }
        }

        /*
            // No need to check challenges if they are all completed
            if (user.Challenges.All(x => x.Value.Complete))
                return;

            foreach (Challenge challenge in user.Challenges
                .Where(x => !x.Value.Complete)
                .Select(x => GetChallenge(x.Key))
                .Where(x => x.Triggers == CriterionTriggers.Game))
            {
                if (challenge.Criterion.Judge(new CriterionContext(user, result)))
                {
                    user.Challenges[challenge.Id].Complete = true;
                }
            }
            */

        public static void SetChallengeProgress(ArcadeUser user, string id, long value)
        {
            if (user.Challenges.Count == 0)
                return;

            // No need to check challenges if they are all completed
            if (user.Challenges.All(x => x.Value.Complete))
                return;

            Challenge challenge = user.Challenges
                .Where(x => !x.Value.Complete)
                .Select(x => GetChallenge(x.Key))
                .FirstOrDefault(x => x.Criterion is VarCriterion v && v.Id == id);

            if (challenge == null)
                return;

            if (challenge.Criterion.Judge(new CriterionContext(user)))
            {
                user.Challenges[challenge.Id].Complete = true;
            }
        }

        public static void AddToChallengeProgress(ArcadeUser user, string id, long amount)
        {
            if (user.Challenges.Count == 0)
                return;

            // No need to check challenges if they are all completed
            if (user.Challenges.All(x => x.Value.Complete))
                return;
        }
    }
}
