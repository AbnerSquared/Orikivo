using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    // used to define the type of key the logger used
    public enum LogKey
    {
        Name = 1,
        Date = 2,
        LogMessage = 3,
        Exception = 4,
        ExceptionType = 5,
        ExceptionMessage = 6,
        LogSeverity = 7,
        LogSource = 8,
        ClientVersion = 9
    }
}
