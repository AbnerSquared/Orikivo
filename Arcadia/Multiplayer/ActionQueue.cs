using System;
using System.Linq;
using System.Threading;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a queued <see cref="GameAction"/>.
    /// </summary>
    public class ActionQueue
    {
        // This is the callback used instead, as it keeps up to date instead of the callback
        private readonly GameSession _session;
        private TimeSpan Duration { get; set; }
        public ActionQueue(TimeSpan duration, string actionId, GameSession session)
        {
            if (session.Actions.All(x => x.Id != actionId))
                throw new Exception($"Expected GameSession to have specified action '{actionId}', but returned null");

            Duration = duration;
            _session = session;
            Id = KeyBuilder.Generate(6);
            StartedAt = DateTime.UtcNow;
            Console.WriteLine($"[{Id}] Timer started at {StartedAt} with elapse action pointing to '{actionId}' in {duration.TotalSeconds} seconds.");
            Timer = new Timer(OnElapse, null, duration, TimeSpan.FromMilliseconds(-1));
            IsCancelled = false;
            IsElapsed = false;
            IsCompleted = false;
            ActionId = actionId;
            EndedAt = null;
        }

        public ActionQueue(string id, TimeSpan duration, string actionId, GameSession session) : this(duration,
            actionId, session)
        {
            Id = id;
            Console.WriteLine($"[{Id}] Timer reference set to {Id}.");
        }

        public string Id { get; }

        internal Timer Timer { get; private set; }
        public bool IsCancelled { get; set; }
        public bool IsElapsed { get; set; }
        public bool IsCompleted { get; set; }
        public string ActionId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? PausedAt { get; private set; }

        public DateTime? EndedAt { get; set; }

        public bool Disposed { get; private set; }

        public void Pause()
        {
            PausedAt = DateTime.UtcNow;
            Console.WriteLine($"[{Id}] Action paused at {PausedAt.Value}");
        }

        public void Resume()
        {
            if (!PausedAt.HasValue)
                return;

            TimeSpan diff = StartedAt - PausedAt.Value;

            PausedAt = null;
            StartedAt = DateTime.UtcNow;
            Duration = Duration.Subtract(diff);
            Timer = new Timer(OnElapse, null, Duration, TimeSpan.FromMilliseconds(-1));
            Console.WriteLine($"[{Id}] Action resumed at {DateTime.UtcNow}");
        }

        public void Cancel()
        {
            if (Disposed)
                return;

            if (IsCancelled)
                return;

            IsCancelled = true;
            EndedAt = DateTime.UtcNow;
            Console.WriteLine($"[{Id}] Action cancelled at {EndedAt.Value}");
        }

        // The state object isn't utilized, so it could be left null
        private void OnElapse(object state)
        {
            if (Disposed)
                return;

            // if the timer was cancelled, just ignore it and set the timer cancellation back to false
            if (IsCancelled)
            {
                Console.WriteLine($"[{Id}] Action has been cancelled and not execute.");
                Dispose();
                return;
            }

            // if the timer was cancelled, just ignore it and set the timer cancellation back to false
            if (PausedAt.HasValue)
            {
                Console.WriteLine($"[{Id}] Action is currently paused.");
                Timer.Dispose();
                Timer = null;
                return;
            }

            IsCompleted = true;
            IsElapsed = true;
            EndedAt = DateTime.UtcNow;
            Logger.Debug($"[{Id}] Action timed out at {EndedAt.Value}. Now executing '{ActionId}'.");
            _session.InvokeAction(ActionId, true);
            IsElapsed = false;
            Dispose();
        }

        public void Dispose()
        {
            if (Disposed)
                return;

            // If no timer is specified, ignore it, but do not mark as disposed since it could be paused
            if (Timer == null)
                return;

            Timer.Dispose();
            Logger.Debug($"[{Id}] Action queue disposed at {DateTime.UtcNow}.");
            _session.ActionQueue.Remove(this);
            Disposed = true;
        }

        public void SafeDispose()
        {
            if (Disposed)
                return;

            // If no timer is specified, ignore it, but do not mark as disposed since it could be paused
            if (Timer == null)
                return;

            Timer.Dispose();
            Logger.Debug($"[{Id}] Action queue disposed at {DateTime.UtcNow}.");
            Disposed = true;
        }

    }
}
