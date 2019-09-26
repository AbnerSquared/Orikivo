using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Cache
{
    public interface IStorageContainer<T>
    {
        void Store(T t, string collection, string key);
    }
}
