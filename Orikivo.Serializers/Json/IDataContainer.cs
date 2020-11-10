using System;
using Orikivo.Models;

namespace Orikivo
{
    public interface IDataContainer<TModel, in TId>
        where TModel : IModel<TId>
    {
        void Add(TModel model);
        void Update(TModel model);
        void AddOrUpdate(TId id, TModel model);
        TModel GetOrAdd(TId id);
        bool TryGet(TId id, out TModel model);
        bool Exists(TId id);
        bool Exists(Func<TModel, bool> predicate);
        TModel RestoreSingle<T>(Func<T, bool> predicate);
    }

    public interface IDataContainer<TModel>
        where TModel : IModel
    {
        void Add(TModel model);
        void Update(TModel model);
        void AddOrUpdate(object id, TModel model);
        TModel GetOrAdd(object id);
        bool TryGet(object id, out TModel model);
        bool Exists(object id);
        bool Exists(Func<TModel, bool> predicate);
        TModel RestoreSingle<T>(Func<T, bool> predicate);
    }
}