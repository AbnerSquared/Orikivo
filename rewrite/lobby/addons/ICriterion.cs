﻿using System.Threading.Tasks;

namespace Orikivo
{
    // criteria to see if all of the values return true
    public interface IOriCriterion<in T>
    {
        Task<bool> JudgeAsync(OriCommandContext sourceContext, T parameter);
    }
}
