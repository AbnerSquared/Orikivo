using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orikivo
{
    // this is essentially a command that can trigger something.
    public class LobbyTrigger<T>
    {
        public LobbyTrigger(string name, bool requireHost = false, bool isLocal = false, bool isTask = false, bool isValid = true, params LobbyTriggerArg[] args)
        {
            Name = name;
            Args = args == null ? null : args.ToList();
            RequireHost = requireHost;
            IsLocal = isLocal;
            Source = new TaskCompletionSource<T>();
            IsValid = isValid;
        }

        public string Name { get; }
        public List<LobbyTriggerArg> Args { get; }
        public bool RequireHost { get; }
        public bool IsValid { get; set; } // if the trigger is valid to execute
        public bool IsLocal { get; }
        public bool IsTask { get; } // if the trigger is a task that executes something outside of a node display
        public TaskCompletionSource<T> Source { get; }

        public void SetResult(T result)
        {
            Source.SetResult(result);
        }
    }
}
