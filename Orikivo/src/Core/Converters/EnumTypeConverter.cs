using Discord.Interactions;
using System;
using System.Collections.Generic;
using Discord;
using System.Threading.Tasks;

namespace Orikivo.Converters
{
    public sealed class EnumTypeConverter<T> : TypeConverter<T> where T : struct, Enum
    {
        public override ApplicationCommandOptionType GetDiscordType()
            => ApplicationCommandOptionType.String;

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            if (Enum.TryParse<T>((string)option.Value, out var result))
                return Task.FromResult(TypeConverterResult.FromSuccess(result));
            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"The specified value could not be converter to {nameof(T)}"));
        }

        public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameter)
        {
            var names = Enum.GetNames(typeof(T));
            if (names.Length <= 25)
            {
                var choices = new List<ApplicationCommandOptionChoiceProperties>();

                foreach (var name in names)
                {
                    choices.Add(new ApplicationCommandOptionChoiceProperties
                    {
                        Name = name,
                        Value = name
                    });
                }

                properties.Choices = choices;
            }
        }
    }
}
