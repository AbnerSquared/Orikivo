using Orikivo.Static;
using Orikivo.Logging;

namespace Orikivo.Utility
{
    public class Debugger
    {
        public static void Write(object obj)
        {
            if (Options.Debug)
            {
                Logger.Log(obj.ToString());
            }
        }
    }
}