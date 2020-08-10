using Orikivo;
using Orikivo.Framework;
using System;
using System.Linq;
using System.Threading;

namespace Arcadia.Multiplayer
{
    /// <summary>
    /// Represents a queued <see cref="GameAction"/>.
    /// </summary>
    public sealed class ActionQueue
    {
        // This is the callback used instead, as it keeps up to date instead of the callback
        private readonly GameSession _session;

        /// <summary>
        /// Initializes a new <see cref="ActionQueue"/> with a generated ID.
        /// </summary>
        /// <param name="duration">The delay at which this <see cref="ActionQueue"/> will be called in.</param>
        /// <param name="actionId">The ID of the <see cref="GameAction"/> to invoke.</param>
        /// <param name="session">The <see cref="GameSession"/> to bind this <see cref="ActionQueue"/> for.</param>
        public ActionQueue(TimeSpan duration, string actionId, GameSession session)
        {
            if (session.Actions.All(x => x.Id != actionId))
                throw new ValueNotFoundException("Failed to find the specified action in the current game session", actionId);

            _session = session;
            CreatedAt = StartedAt = DateTime.UtcNow;
            Id = KeyBuilder.Generate(6);
            ActionId = actionId;
            Delay = duration;
            Timer = new Timer(OnElapse, null, duration, TimeSpan.FromMilliseconds(-1));
            Logger.Debug($"[{Id}] Queued '{actionId}' (at {Format.FullTime(StartedAt)}) to invoke in {Format.Countdown(duration)}.");
        }

        /// <summary>
        /// Initializes a new <see cref="ActionQueue"/> with a unique ID.
        /// </summary>
        /// <param name="id">The ID to set for this <see cref="ActionQueue"/>.</param>
        /// <param name="duration">The delay at which this <see cref="ActionQueue"/> will be called in.</param>
        /// <param name="actionId">The ID of the <see cref="GameAction"/> to invoke.</param>
        /// <param name="session">The <see cref="GameSession"/> to bind this <see cref="ActionQueue"/> for.</param>
        public ActionQueue(string id, TimeSpan duration, string actionId, GameSession session)
        {
            if (session.ActionQueue.Any(x => x.Id == id))
                throw new Exception($"There is already a queue with the specified ID ('{id}').");

            if (session.Actions.All(x => x.Id != actionId))
                throw new ValueNotFoundException("Failed to find the specified action in the current game session", actionId);

            _session = session;
            CreatedAt = StartedAt = DateTime.UtcNow;
            Id = id;
            ActionId = actionId;
            Delay = duration;
            Timer = new Timer(OnElapse, null, duration, TimeSpan.FromMilliseconds(-1));
            Logger.Debug($"[{Id}] Queued '{actionId}' (at {Format.FullTime(StartedAt)}) to invoke in {Format.Countdown(duration)}.");
        }

        public string Id { get; }

        public string ActionId { get; }

        private Timer Timer { get; set; }

        /// <summary>
        /// Determines if this <see cref="ActionQueue"/> was cancelled.
        /// </summary>
        public bool IsCancelled { get; private set; }

        /// <summary>
        /// Determines if this <see cref="ActionQueue"/> was already completed.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Determines if this <see cref="ActionQueue"/> is currently invoking the specified action.
        /// </summary>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which this <see cref="ActionQueue"/> was initialized.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which this <see cref="ActionQueue"/> started.
        /// </summary>
        public DateTime StartedAt { get; private set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which this <see cref="ActionQueue"/> was paused, if any.
        /// </summary>
        public DateTime? PausedAt { get; private set; }

        private TimeSpan Delay { get; set; }

        /// <summary>
        /// Determines if this <see cref="ActionQueue"/> was disposed.
        /// </summary>
        public bool Disposed { get; private set; }

        public void Pause()
        {
            if (PausedAt.HasValue)
                return;

            PausedAt = DateTime.UtcNow;
            Logger.Debug($"[{Id}] Action paused at {PausedAt.Value}");
        }

        public void Resume()
        {
            if (!PausedAt.HasValue)
                return;

            TimeSpan diff = StartedAt - PausedAt.Value;
            PausedAt = null;
            StartedAt = DateTime.UtcNow;
            Delay = Delay.Subtract(diff);
            Timer = new Timer(OnElapse, null, Delay, TimeSpan.FromMilliseconds(-1));
            Logger.Debug($"[{Id}] Queue timer resumed");
        }

        public void Cancel()
        {
            if (Disposed)
                return;

            if (IsCancelled)
                return;

            IsCancelled = true;
            Logger.Debug($"[{Id}] Queue cancelled");
        }

        // NOTE: The state object parameter isn't utilized for this class, and is left null
        private void OnElapse(object state)
        {
            if (Disposed)
                return;

            if (IsCancelled)
            {
                Logger.Debug($"[{Id}] Queue called but was cancelled");
                Dispose();
                return;
            }

            if (PausedAt.HasValue)
            {
                Logger.Debug($"[{Id}] Queue called but is paused");
                Timer.Dispose();
                Timer = null;
                return;
            }

            IsCompleted = true;
            IsBusy = true;

            Logger.Debug($"[{Id}] Queue called, executing action '{ActionId}'");
            _session.InvokeAction(ActionId, true);
            IsBusy = false;

            Logger.Debug($"[{Id}] Queue call complete, now disposing");
            Dispose();
        }

        /// <summary>
        /// Disposes of this <see cref="ActionQueue"/>.
        /// </summary>
        public void SafeDispose()
        {
            if (Disposed)
                return;

            if (Timer == null)
            {
                // ReSharper disable once InvertIf
                if (PausedAt.HasValue)
                {
                    Logger.Debug($"[{Id}] Queue call was paused but disposed");
                    Disposed = true;
                }
                return;
            }

            Timer.Dispose();
            Disposed = true;

            Logger.Debug($"[{Id}] Queue call {(IsCompleted ? "successfully " : "")}disposed.");
        }

        /// <summary>
        /// Disposes of this <see cref="ActionQueue"/> and removes it from the <see cref="GameSession"/> cache.
        /// </summary>
        public void Dispose()
        {
            SafeDispose();
            _session.ActionQueue.Remove(this);
        }
    }
}
