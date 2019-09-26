using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Orikivo
{
    public class OriHelperServiceTests
    {
        public static async Task<string> CompileTestAsync(string context)
        {
            OriHelpService oriHelperService = new OriHelpService(null);
            ContextInfo ctx = ContextInfo.Parse(context);
            oriHelperService.Dispose();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("```bf");

            sb.AppendLine("test_complete::");
            sb.AppendLine($"ctx >> {ctx.Content}");
            sb.AppendLine($"ctx.success = {ctx.IsSuccess.ToString().ToLower()};");

            if (ctx.Modules != null)
                sb.AppendLine($"ctx.modules = [{string.Join(',', ctx.Modules)}];");

            if (ctx.Groups != null)
                sb.AppendLine($"ctx.groups = [{string.Join(',', ctx.Groups)}];");

            if (ctx.HasRoot)
                sb.AppendLine($"ctx.root = {ctx.Root};");

            if (ctx.Type != null)
                sb.AppendLine($"ctx.type = {ctx.Type.ToString().ToLower()};");

            if (ctx.HasPriority)
                sb.AppendLine($"ctx.overload = {ctx.Priority};");

            if (ctx.SearchFormat != null)
                sb.AppendLine($"ctx.searchFormat = {ctx.SearchFormat.ToString().ToLower()};");

            if (ctx._index.HasValue)
                sb.AppendLine($"ctx.index = {ctx.Index};");

            if (ctx.HasArg)
                sb.AppendLine($"ctx.arg = {ctx.Arg};");

            sb.Append("```");

            return sb.ToString();

        }
    }
}
