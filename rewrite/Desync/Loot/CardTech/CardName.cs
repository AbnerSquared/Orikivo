using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class CardName
    {
        public string DesignId { get; set; }

        public int? FontId { get; set; } // Leave empty for default
        public List<int> FontModifierIds { get; set; } // leave empty for nothing

        public int? Gamma { get; set; }
        public int? LocalColorId { get; set; }
        public bool Active { get; set; }
    }

    // loot is stuff like:
    // - sockets, which enhance your digital features
    // - new backpacks to store more stuff at a time
    // - transportation devices, which speed up travel time between places
    // - collectables in the real world, which can be traded for valuables in the digital
    // - CardTech, which enhances and modifies how your card is displayed
    //   CardTech items: // By default, your card color scheme is bound to your interface color scheme, unless you explicitly modify it
    //   - Color Schemes (Bind IDs to GammaColorMap)
    //   - Font faces (Bind IDs to FontFace)
    //   - Avatar animator (Simply a flag)
    //   - New layout templates, which change how your card is organized. (you can unlock an advanced card layout concept, which allows you to set your card to literally anything
    //   - Avatar canvas, which allows you to instead draw your avatar
    //   - Avatar size regulator, which allows you to modify the avatar appearance size on your card.
    //   - Status Templates, which change how your status is displayed color-wise
    //   - Level Templates, which change how your level is displayed
    //   - Exp Templates, which change how your current experience is displayed (exp template can be set referencing another template)
    //   - Username templates, which change how your name is displayed
    //   - Font Face Modifiers, which change how the font is written
    //   - Backgrounds, which are displayed behind a card.
    //   - Merit Slots, which allow you to display the icon of a merit onto a card


    // - Interface Enhancements, which modifies how Orikivo visually displays content to you
}
