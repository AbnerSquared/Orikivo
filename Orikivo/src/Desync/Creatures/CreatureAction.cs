namespace Orikivo.Desync
{
    // might be an enum instead
    public enum CreatureAction
    {
        Idle = 1, // the creature remains still
        Attack = 2, // the creature attacks the user
        Run = 3, // the creature will attempt to run away from the user
        Defend = 4 // the creature will attempt to defend itself from an attack.
    }

}
