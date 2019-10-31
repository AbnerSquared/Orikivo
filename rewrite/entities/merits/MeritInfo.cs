namespace Orikivo
{
    // TODO: Calculate % of all users that have unlocked this merit.
    public class MeritInfo
    {
        public string Name { get; }
        public string Id { get; }
        public MeritGroup Group { get; } = MeritGroup.Misc;
        public VarCriterion[] Criteria { get; }
        public MeritRewardInfo OnSuccess { get; }
    }
}
