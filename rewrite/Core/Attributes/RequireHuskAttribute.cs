using Discord.Commands;
using Orikivo.Unstable;
using System;
using System.Threading.Tasks;

namespace Orikivo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]  
    public class CheckFlagsAttribute : PreconditionAttribute
    {
        public Gate Gate { get; }
        public string[] Ids { get; }

        public CheckFlagsAttribute(Gate type, params string[] ids)
        {
            Gate = type;
            Ids = ids;
        }

        // allows you to check without preconditions
        public bool Check(HuskBrain brain)
        {
            bool match = Gate == Gate.HAS;
            foreach (string id in Ids)
            {
                bool hasFlag = brain.HasFlag(id);

                if (match != hasFlag)
                    return false;
            }

            return true;
        }

        // TODO: These commands should be completely hidden from use if the criteria doesn't meet.
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            OriCommandContext Context = context as OriCommandContext;
            Context.Container.GetOrAddUser(Context.User);

            if (!Check(Context.Account.Brain))
                return PreconditionResult.FromError("Husk criteria not met.");

            return PreconditionResult.FromSuccess();
        }
    }
}
