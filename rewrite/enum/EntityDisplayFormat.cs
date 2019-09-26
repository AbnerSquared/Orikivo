using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // simplify the name EntityFormat
    // this is used to determine how accounts are rendered. (i.e. guild, user)
    public enum EntityDisplayFormat
    {
        Text = 1, // text display
        Json = 2, // code markdown display
        //Embed = 3, // embed display
        //Render = 4 // pixel art display
    }
}
