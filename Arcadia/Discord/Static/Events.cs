using System;
using Orikivo;

namespace Arcadia
{
    internal static class Events
    {
        internal static readonly DateTimeRange Halloween = new DateTimeRange(new DateTime(0, 10, 10), new DateTime(0, 11, 1));
        internal static readonly DateTimeRange Christmas = new DateTimeRange(new DateTime(0, 12, 5), new DateTime(0, 12, 26));
    }
}
