using System.Collections.Generic;

namespace Orikivo
{
    public interface ICasinoResult
    {
        List<StatUpdateInfo> StatsToChange { get; set; }
    }
}
