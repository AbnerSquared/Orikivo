using System.Collections.Generic;

namespace Orikivo
{
    public interface ILogReader
    {
        IEnumerable<string> GetLogs(int amount = 1);
    }
}