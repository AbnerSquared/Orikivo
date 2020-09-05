namespace Orikivo.Desync
{
    public enum ClaimRewardMethod
    {
        /// <summary>
        /// Marks the <see cref="Claimable"/> to loop its rewards.
        /// </summary>
        Interval = 1,

        /// <summary>
        /// Marks the <see cref="Claimable"/> to allow rewards only once.
        /// </summary>
        Once = 2
    }
}