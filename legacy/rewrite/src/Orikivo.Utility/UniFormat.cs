using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Utility
{
    // all unicode formatting
    public static class UniFormat
    {
        public static string Bold(string value, CaseFormat caseFormat = CaseFormat.Ignore)
            => value.ToBold(caseFormat);
    }
}
