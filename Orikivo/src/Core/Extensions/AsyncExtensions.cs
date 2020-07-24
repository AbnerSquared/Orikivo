using System;
using System.Threading.Tasks;
// ReSharper disable SuggestVarOrType_Elsewhere

namespace Orikivo
{
    // REF: Discord.Net.Utils.AsyncEvent.cs
    public static class AsyncExtensions
    {
        public static async Task InvokeAsync(
            this AsyncEvent<Func<Task>> handler)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke().ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T>(
            this AsyncEvent<Func<T, Task>> handler,
            T arg)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2>(
            this AsyncEvent<Func<T1, T2, Task>> handler,
            T1 arg1, T2 arg2)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3>(
            this AsyncEvent<Func<T1, T2, T3, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4>(
            this AsyncEvent<Func<T1, T2, T3, T4, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10)
                    .ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11)
                    .ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11,
            T12 arg12)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12)
                    .ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11,
            T12 arg12, T13 arg13)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,
                    arg13).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11,
            T12 arg12, T13 arg13, T14 arg14)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,
                    arg13, arg14).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11,
            T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,
                    arg13, arg14, arg15).ConfigureAwait(false);
        }

        public static async Task InvokeAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            this AsyncEvent<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task>> handler,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11,
            T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            foreach (var t in handler.Subscriptions)
                await t.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12,
                    arg13, arg14, arg15, arg16).ConfigureAwait(false);
        }
    }
}
