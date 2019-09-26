using System;

namespace Orikivo
{
    // an itemobject is a store item, holds the name, icon, and cost.
    // Color schemes
    // Borders
    // Backgrounds
    // KeyItems
    public interface ItemObject : IEquatable<ItemObject>
    {
        string Name {get;}
        ulong Cost {get;}
        //string Icon {get;}
    }
}