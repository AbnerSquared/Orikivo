using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public enum ReportFlag
    {
        Suggestion = 1, // ideas/suggestions
        Visual = 2, // visual errors
        Runtime = 3, // incorrect functions
        Exception = 4, // auto-exception reports
        Critical = 5 // major issues
    }
}
