using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace Orikivo
{
    // marks a command to require an attachment
    // an attachment could be another argument.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RequireAttachmentAttribute : PreconditionAttribute
    {
        public string Name { get; }
        public ExtensionType Type { get; }
        public RequireAttachmentAttribute(ExtensionType format = ExtensionType.Text, string name = "attachment")
        {
            Type = format;
            Name = name;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var Context = context as DesyncContext;

            if (Context.Message.Attachments.Count > 0)
                if (Context.Message.Attachments.Any(x => MatchesExtension(Type, x.Filename)))
                    return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError($"You are missing a required attachment that requires a format of '{Type.ToString()}'.");
        }

        private static bool MatchesExtension(ExtensionType type, string path)
        {
            string extension = Path.GetExtension(path)?.ToLower();
            bool isNull = string.IsNullOrWhiteSpace(extension);

            if (type.HasFlag(ExtensionType.Empty))
                return isNull;
            else if (type.HasFlag(ExtensionType.Any))
                return !isNull;
            else
                return GetAllowedExtensions(type)?.Contains(extension) ?? false;
        }

        private static IEnumerable<string> GetAllowedExtensions(ExtensionType type)
        {
            var extensions = new List<string>();

            if (type.HasFlag(ExtensionType.Any | ExtensionType.Empty))
                return null;

            foreach (ExtensionType activeType in type.GetActiveFlags())
                extensions.Add('.' + activeType.ToString().ToLower());

            return extensions;
        }
    }
}
