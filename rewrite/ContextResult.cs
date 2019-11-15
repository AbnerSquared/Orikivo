using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    public struct ContextResult
    {
        private ContextResult(ContextInfoType? type, ModuleInfo module = null, ParameterInfo parameter = null, IEnumerable<CommandInfo> commands = null, OriGuild guild = null, GuildCommand custom = null, ContextError? error = null, string errorReason = null)
        {
            Type = type;
            Module = module;
            Parameter = parameter;
            Commands = commands;
            Guild = guild;
            Custom = custom;
            Error = error;
            ErrorReason = errorReason;
        }

        public ContextInfoType? Type { get; }
        public ModuleInfo Module { get; } // Matching module / group
        public ParameterInfo Parameter { get; } // Matching parameter
        
        public IEnumerable<CommandInfo> Commands { get; } // Matching command(s)

        public OriGuild Guild { get; }

        public GuildCommand Custom { get; }

        public bool IsSuccess => !Error.HasValue;

        public string ErrorReason { get; }

        public ContextError? Error { get; }

        public static ContextResult FromSuccess(OriGuild guild)
            => new ContextResult(ContextInfoType.Guild, guild: guild);

        public static ContextResult FromSuccess(GuildCommand custom)
            => new ContextResult(ContextInfoType.Custom, custom: custom);

        public static ContextResult FromSuccess(ModuleInfo module)
            => new ContextResult(Checks.NotNull(module.Group) ? ContextInfoType.Group : ContextInfoType.Module, module);

        public static ContextResult FromSuccess(IEnumerable<CommandInfo> commands)
            => new ContextResult(commands.Count() > 1 ? ContextInfoType.Command : ContextInfoType.Overload, commands: commands);

        public static ContextResult FromSuccess(ParameterInfo parameter)
            => new ContextResult(ContextInfoType.Parameter, parameter: parameter);

        public static ContextResult FromError(ContextError error, string errorReason = null)
            => new ContextResult(null, error: error, errorReason: errorReason);
    }
}
