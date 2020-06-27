using System;
using System.Threading;
using Orikivo;

namespace Arcadia
{
    public class ActionQueue
    {
        private readonly GameSession _session;
        public ActionQueue(TimeSpan duration, string actionId, GameSession session)
        {
            _session = session;
            Id = KeyBuilder.Generate(6);
            StartedAt = DateTime.UtcNow;
            Console.WriteLine($"[{Id}] Timer started at {StartedAt} with elapse action pointing to '{actionId}' in {duration.TotalSeconds} seconds.");
            Timer = new Timer(OnElapse, session, duration, TimeSpan.FromMilliseconds(-1));
            IsCancelled = false;
            IsElapsed = false;
            IsCompleted = false;
            ActionId = actionId;
            EndedAt = null;
        }

        public string Id { get; }

        internal Timer Timer { get; }
        public bool IsCancelled { get; set; }
        public bool IsElapsed { get; set; }
        public bool IsCompleted { get; set; }
        public string ActionId { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public bool Disposed { get; private set; } = false;

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

        private void OnElapse(object state)
        {
            if (Disposed)
                return;

            GameSession session = state as GameSession;

            // if the timer was cancelled, just ignore it and set the timer cancellation back to false
            if (IsCancelled)
            {
                Console.WriteLine($"[{Id}] Action has been cancelled and not execute.");
                Dispose();
                return;
            }

            IsCompleted = true;
            IsElapsed = true;
            EndedAt = DateTime.UtcNow;
            Console.WriteLine($"[{Id}] Action timed out at {EndedAt.Value}. Now executing '{ActionId}'.");
            _session.InvokeAction(ActionId, true);
            IsElapsed = false;
            Dispose();
        }

        public void Dispose()
        {
            Timer.Dispose();
            
            Console.WriteLine($"[{Id}] Action queue disposed at {DateTime.UtcNow}.");
        }

    }
}
