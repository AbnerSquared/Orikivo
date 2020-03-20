namespace Orikivo.Desync
{
    public enum CreatureArchetype
    {

        Neutral = 1, // a neutral creature, won't attack unless attacked
        Timid = 2, // a creature that tends to run away often
        Hostile = 3, // a creature that will attack every chance it gets
        Aggressive = 4, // a creature that is neutral, but will be ruthless when provoked
        Sturdy = 5, // a creature that won't do much, but can reach a breaking point given enough pushing
    }

}
