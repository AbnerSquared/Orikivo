namespace Orikivo.Desync
{
    // Topic is what stores what the dialog is about

    // Listener stores who the player is that is eligble to receive this dialog
    // this can set required flags, relationship levels, etc.

    // tailor only stores how an NPC speaks
    public class DialogTailor
    {
        MindType? Mind { get; set; }
        EnergyType? Energy { get; set; }

        NatureType? Nature { get; set; }

        TacticType? Tactic { get; set; }
        IdentityType? Identity { get; set; }
    }

}
