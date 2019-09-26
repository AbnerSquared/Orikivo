using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orikivo
{
    public static class IServiceCollectionExtension
    {
        public static void AddSingletons(this IServiceCollection collection, UnitManager m)
            => m.AddSingletonsToService(collection);
    }
}