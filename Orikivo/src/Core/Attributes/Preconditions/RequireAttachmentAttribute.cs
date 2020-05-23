using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // marks a command to require an attachment
    // an attachment could be another argument.
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RequireAttachmentAttribute : PreconditionAttribute
    {
        public string Name { get; }
        public FileFormat Format { get; }
        public RequireAttachmentAttribute(FileFormat format = FileFormat.Text, string name = "attachment")
        {
            Format = format;
            Name = name;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            OriCommandContext Context = context as OriCommandContext;

            if (Context.Message.Attachments.Count > 0)
                if (Context.Message.Attachments.Any(x => MatchesFormat(Format, x.Filename)))
                    return PreconditionResult.FromSuccess();

            return PreconditionResult.FromError($"You are missing a required attachment that requires a type of '{Format.ToString()}'.");
        }

        private static bool MatchesFormat(FileFormat format, string path)
            => GetExtension(format) == Path.GetExtension(path).ToLower();
                

        private static string GetExtension(FileFormat format)
            => format switch
            {
                FileFormat.Text => ".txt",
                _ => throw new ArgumentException("Invalid file format specified.")
            };
    }
}
