using System.Threading.Tasks;
using Orikivo.Systems;

namespace Orikivo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*
            Task.Run(async() =>
            {
                await UnitManager.CreateInstance(args);
            }).GetAwaiter().GetResult();
            */
            Task.Run(async() =>
            {
                await UnitCollection.CollectServiceAsync(args);
            }).GetAwaiter().GetResult();
        }
    }
}