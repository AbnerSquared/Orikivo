using System;
using System.Collections.Generic;

namespace Orikivo
{
    public interface IStorable : IEquatable<IStorable>
    {
        ulong Id { get; }
        bool Updated { get; }
        bool Building { get; }
        void UpdateCard();
        void Save();
    }
}