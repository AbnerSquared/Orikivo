namespace Orikivo.Desync
{
    // might be an enum instead
    public enum ActionType
    {
        Static = 1, // the creature remains still
        // keeps the battle going

        Attack = 2, // this action will be inflicting damage
        // keeps the battle going

        Run = 3, // this action will focus on moving away
        // can end a battle.
    }

}
