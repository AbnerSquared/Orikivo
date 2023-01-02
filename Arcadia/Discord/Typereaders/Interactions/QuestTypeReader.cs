using System;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace Arcadia.Interactions
{
    public sealed class QuestTypeReader : TypeReader
    {
        public override bool CanConvertTo(Type type)
        {
            return typeof(Quest).IsAssignableFrom(type);
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext ctx, string input, IServiceProvider provider)
        {
            if (QuestHelper.Exists(input))
                return Task.FromResult(TypeConverterResult.FromSuccess(QuestHelper.GetQuest(input)));

            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Could not find a Quest with the specified ID."));
        }
    }
}
