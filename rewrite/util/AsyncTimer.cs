﻿using System;
using System.Threading.Tasks;

namespace Orikivo
{
    // TODO: Rework this class to allow for TickUpdatePacket and such.
    public class AsyncTimer
    {
        public AsyncTimer(TimeSpan duration)
        {
            Duration = duration;
            Trigger = false;
        }

        public TimeSpan Duration { get; set; }
        private bool Trigger { get; set; }
        public bool ForceQuit { get; set; }
        public bool Running { get; private set; }
        public void Reset()
        {
            Trigger = true;
        }


        // TODO: Remove 'goto' label.
        // implement user count into lobby as a part of the timer mechanic.
        public async Task<bool> StartAsync()
        {
            Running = true;
            Task _timer = Task.Delay(Duration);
            Entry:
            while (!Task.WhenAll(_timer).ConfigureAwait(false).GetAwaiter().IsCompleted)
            {
                if (ForceQuit)
                {
                    Running = false;
                    ForceQuit = false;
                    return true;
                }
                if (Trigger)
                {
                    Console.WriteLine("[Debug] -- Timer reset triggered. --");
                    Trigger = false;
                    _timer = Task.Delay(Duration);
                    goto Entry;
                }
            }

            Running = false;
            return true;
        }
    }
}