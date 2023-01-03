using System;

namespace Arcadia
{
    public interface ICriterion<in TContext>
    {
        Func<TContext, bool> OnTrigger { get; }
    }
}
