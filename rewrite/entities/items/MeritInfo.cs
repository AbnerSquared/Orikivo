using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Create merit info stuff. It requires a Name, ID, Reward, and Criteria.
    public class MeritInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public List<AccountCriterion> Criteria { get; set; }
        public MeritRewardInfo OnSuccess { get; set; }
    }


    // OriUser based criterion check.
    public class AccountCriterion
    {
        public string Id { get; } // the id of the attribute.
        public int Value { get; } // the value this specified attribute should be at.
    }

    /// <summary>
    /// Represents a generic achievement.
    /// </summary>
    public interface IMerit
    {
        /// <summary>
        /// The unique identifier for this merit.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The name of this merit.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The group that this merit is bound to, if specified.
        /// </summary>
        MeritGroup? Group { get; }

        /// <summary>
        /// The criteria that an account needs to meet to earn this merit.
        /// </summary>
        IReadOnlyList<AccountCriterion> Criteria { get; }

        /// <summary>
        /// The rewards given to an account upon meeting the criteria.
        /// </summary>
        MeritRewardInfo OnSuccess { get; }
    }

    // the overall group of the merit specified.
    public enum MeritGroup
    {

    }
}
