using System;
using System.Threading.Tasks;

namespace Orikivo
{
    // refer to Discord.Net.Utils
    internal static class AsyncExtensions
    {
        public static async Task InvokeAsync(this AsyncEvent<Func<Task>> handler)
        {
            var subscribers = handler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke().ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T>(this AsyncEvent<Func<T, Task>> handler, T arg)
        {
            var subscribers = handler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2>(this AsyncEvent<Func<T1, T2, Task>> handler, T1 arg1, T2 arg2)
        {
            var subscribers = handler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3>(this AsyncEvent<Func<T1, T2, T3, Task>> handler, T1 arg1, T2 arg2, T3 arg3)
        {
            var subscribers = handler.Subscriptions;
            for (int i = 0; i < subscribers.Count; i++)
                await subscribers[i].Invoke(arg1, arg2, arg3).ConfigureAwait(false);
        }
    }
}
