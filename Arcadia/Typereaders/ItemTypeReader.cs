using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Arcadia
{
    // Once ready, you can then reference the command context to load the item information
    public sealed class ItemTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (ItemHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(ItemHelper.GetItem(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find an Item with the specified ID."));
        }
    }

    public sealed class MeritTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (MeritHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(MeritHelper.GetMerit(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Merit with the specified ID."));
        }
    }

    public sealed class RecipeTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (ItemHelper.RecipeExists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(ItemHelper.GetRecipe(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Recipe with the specified ID."));
        }
    }

    public sealed class QuestTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (QuestHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(QuestHelper.GetQuest(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Quest with the specified ID."));
        }
    }

    public sealed class ShopTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext ctx, string input, IServiceProvider provider)
        {
            if (ShopHelper.Exists(input))
                return Task.FromResult(TypeReaderResult.FromSuccess(ShopHelper.GetShop(input)));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Could not find a Shop with the specified ID."));
        }
    }
}
