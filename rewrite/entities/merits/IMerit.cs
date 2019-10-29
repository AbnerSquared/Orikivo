using System.Collections.Generic;

namespace Orikivo
{
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
        List<AccountCriterion> Criteria { get; }

        /// <summary>
        /// The rewards given to an account upon meeting the criteria.
        /// </summary>
        MeritRewardInfo OnSuccess { get; }
    }
}
