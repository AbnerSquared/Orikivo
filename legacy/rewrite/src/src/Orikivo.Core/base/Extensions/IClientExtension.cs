using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Orikivo.Storage;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orikivo.Networking;
using Orikivo.Modules;
using System.Text;

namespace Orikivo
{
    public static class SocketClientExtension
    {
        public static bool TryGetModule(this IEnumerable<ModuleInfo> modules, string name, out ModuleInfo module)
        {
            module = modules.FirstOrDefaultModule(name);
            if (!module.Exists())
            {
                return false;
            }

            return true;
        }

        public static bool IsActive(this ModuleInfo module, IEnumerable<ModuleInfo> current)
        {
            return current.Contains(module);
        }

        public static ModuleInfo FirstOrDefaultModule(this IEnumerable<ModuleInfo> modules, string name)
        {
            if (modules.TryGetModules(name, out IEnumerable<ModuleInfo> all))
            {
                return all.FirstOrDefault();
            }
            return null;
        }

        public static CommandInfo FirstOrDefaultCommand(this IEnumerable<CommandInfo> commands, string name)
        {
            if (commands.TryGetCommands(name, out IEnumerable<CommandInfo> all))
            {
                return all.FirstOrDefault();
            }
            return null;
        }

        public static bool HasMatchingName(this ModuleInfo module, string name)
        {
            return module.Name.ToLower() == name.ToLower();
        }

        public static bool HasMatchingAliases(this ModuleInfo m, string name)
        {
            bool groupmatch = false;
            if (m.Group.Exists())
            {
                if (m.Group.ToLower() == name.ToLower())
                {
                    groupmatch = true;
                }
            }

            return m.HasMatchingName(name) || groupmatch || m.Aliases.Any(x => x.ToLower() == name.ToLower());
        }

        public static bool TryGetModules(this IEnumerable<ModuleInfo> list, string name, out IEnumerable<ModuleInfo> modules)
        {
            modules = null;
            if (list.Any(x=> x.StartsWith(name)))
            {
                modules = list.Where(x => x.StartsWith(name));
                return true;
            }

            return false;
        }
        public static IEnumerable<ArgumentInfo> TryGetArgs(this CommandInfo command)
        {
            List<ArgumentInfo> args = new List<ArgumentInfo>();
            if (!command.Parameters.Funct())
            {
                return null;
            }
            foreach(ParameterInfo param in command.Parameters)
            {
                args.Add(param.ToArgument());
            }

            return args;
        }

        public static ArgumentInfo ToArgument(this ParameterInfo param)
        {
            return new ArgumentInfo(param.Name, param.Type, param.IsOptional, param.GetSummary());
        }

        public static ArgumentSummary GetSummary(this ParameterInfo param)
        {
            return new ArgumentSummary(param.Summary ?? "no summary.");
        }

        public static string GetName(this CommandInfo command)
        {
            return command.Name ?? command.ToString();
        }

        public static CommandSyntax GetSyntax(this CommandInfo command)
        {
            // connect with Global.Modules.Commands.Syntax
            IEnumerable<ArgumentInfo> args = command.TryGetArgs();
            return new CommandSyntax(command.GetFullName(), command.GetName(), args);
        }

        public static bool Funct<T> (this IEnumerable<T> list)
        {
            if (!list.Exists())
            {
                return false;
            }
            if (!(list.Count() > 0))
            {
                return false;
            }
            return true;
        }

        public static bool StartsWith(this ModuleInfo module, string name)
        {
            return module.GetName().ToLower().StartsWith(name);
        }

        public static bool StartsWith(this CommandInfo command, string name)
        {
            return command.Name.ToLower().StartsWith(name);
        }

        public static bool ContainsString(this CommandInfo command, string name)
        {
            return command.Name.ToLower().Contains(name);
        }

        public static string TryGetName(this CommandInfo command)
        {
            if (command.GetName() == "")
            {
                if (command.Module.Group.Exists())
                {
                    return command.Module.Group;
                }
                else
                {
                    return "";
                }
            }
            return command.GetName();
        }

        public static string GetFullName(this CommandInfo command)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('.');
            sb.Append(command.TryGetName());

            ModuleInfo m = command.Module;
            List<string> list = new List<string>();
            list.Add(m.GetName());
            while(m.Parent.Exists())
            {
                m = m.Parent;
                list.Add(m.GetName());
            }

            sb.Insert(0, string.Join('.', list));
            return sb.ToString();
        }

        public static bool TryGetCommands(this CommandService service, string name, out IEnumerable<CommandInfo> commands)
        {
            commands = null;
            if (service.Commands.Any(x => x.HasMatchingName(name)))
            {
                commands = service.Commands.Where(x => x.StartsWith(name));
                return true;
            }

            return false;
        }

        public static bool TryGetCommands(this IEnumerable<CommandInfo> cmds, string name, out IEnumerable<CommandInfo> commands)
        {
            commands = null;
            if (cmds.Any(x => x.HasMatchingName(name)))
            {
                commands = cmds.Where(x => x.StartsWith(name));
                return true;
            }

            return false;
        }

        public static string ToPrimaryUpper(this string s)
        {
            char[] chrs = s.ToLower().ToCharArray();
            chrs[0] = chrs[0].ToUpper();
            return new string(chrs);
        }

        public static char ToUpper(this char c)
        {
            return char.ToUpper(c);
        }


        public static bool TryGetCommand(this CommandService service, string name, out CommandInfo command)
        {
            command = null;
            if (service.TryGetCommands(name, out IEnumerable<CommandInfo> matches))
            {
                command = matches.Where(x => x.HasMatchingName(name)).First();
                return true;
            }

            return false;
        }

        public static bool HasMatchingName(this CommandInfo command, string name)
        {
            return command.Name.ToLower() == name.ToLower();
        }

        public static bool HasMatchingAliases(this CommandInfo command, string name)
        {
            return command.Aliases.Any(x => x.ToLower() == name.ToLower());
        }



        public static List<SocketUser> SortByName(this IReadOnlyCollection<SocketUser> users) =>
            users.OrderBy(x => x.Username).ToList();

        public static List<SocketGuild> SortByName(this IReadOnlyCollection<SocketGuild> guilds) =>
            guilds.OrderBy(x => x.Name).ToList();

        public static bool TryGetGuild(this OrikivoCommandContext ctx, string s, out SocketGuild g)
        {
            g = null;
            if (!s.Exists())
            {
                return false;
            }
            g = ctx.Client.GetGuild(s);
            if (!g.Exists())
            {
                return false;
            }

            return true;
        }

        public static bool TryGetGuild(this OrikivoCommandContext ctx, ulong u, out SocketGuild g)
        {
            g = ctx.Client.GetGuild(u);
            if (!g.Exists())
            {
                return false;
            }

            return true;
        }

        public static string GetName(this ModuleInfo module)
        {
            return module.Name ?? module.ToString();
        }

        public static SocketGuild FirstOrDefaultGuild(this OrikivoCommandContext context, string s) =>
            s.Exists() ? context.Client.GetGuild(s) : context.Guild;
        
        public static SocketGuild GetGuild(this DiscordSocketClient client, string s) =>
            ulong.TryParse(s, out ulong id) ? client.Guilds.FirstOrDefault(x => x.Id == id) : client.Guilds.FirstOrDefault(x => x.Name == s);

        public static async Task UpdateActivity(this DiscordSocketClient c, CompactActivity a)
        {
            await c.SetGameAsync(a.Name, type: a.Type);
        }
    }
}