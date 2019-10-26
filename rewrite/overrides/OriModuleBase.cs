using Discord.Commands;

namespace Orikivo
{
    // based off of Discord.Addons.Interactive
    // extending modulebase to support dynamic events
    public abstract class OriModuleBase<T> : ModuleBase<T>
        where T : OriCommandContext
    {
        // TODO: Create singular message handles, like the one in game client.
    }
}
