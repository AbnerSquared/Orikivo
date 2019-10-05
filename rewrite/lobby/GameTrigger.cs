using System;

namespace Orikivo
{
    public class GameTrigger
    {
        public GameTrigger(string name, GameTriggerArg arg = null, GameAttributeUpdate onSuccess = null)
        {
            Name = name; // the name that is used to match with a trigger.
            Arg = arg; // the arg that is required whenever 
            OnSuccess = onSuccess; // to execute whenever the trigger is activated.
        }

        public string Name { get; }
        public GameTriggerArg Arg { get; }
        public GameAttributeUpdate OnSuccess { get; }
        public bool RequiresArg => !(Arg == null);

        public bool Parse(string context)
        {
            // create a regex here
            // check if the pattern used is a success
            // if the arg does exist
            // try to get the valid object type from the one specified.
            // if onsuccess is specified
            throw new NotImplementedException();
        }
    }
}
