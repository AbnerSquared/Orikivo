using Arcadia.Models;
using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcadia.Autocompletion
{
    public sealed class SeenItemAutocompleteHandler : AutocompleteHandler
    {
        public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext ctx, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            if (!(ctx is ArcadeInteractionContext context))
                return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "The specified context is incorrect");

            if (context.Account == null)
                return AutocompletionResult.FromError(InteractionCommandError.Unsuccessful, "You do not have an account");

            IEnumerable<IModel<string>> items = CatalogHelper.GetSeenItems(context.Account);
            string current = autocompleteInteraction.Data.Current.Value.ToString();

            items = CollectionHelper.OrderByScore(current, items);

            IEnumerable<AutocompleteResult> results = items.Select(x => new AutocompleteResult(x.Name, x.Id));
            return AutocompletionResult.FromSuccess(results.Take(25));
        }
    }
}
