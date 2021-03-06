﻿using System;
using System.Diagnostics;
using System.Threading;
using HA4IoT.Contracts.Core;
using HA4IoT.Contracts.Logging;

namespace HA4IoT.Core.Timer
{
    public class HomeAutomationTimer : IHomeAutomationTimer
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public event EventHandler<TimerTickEventArgs> Tick;
        
        public TimeSpan CurrentTime => CurrentDateTime.TimeOfDay;

        public DateTime CurrentDateTime => DateTime.Now;

        public TimedAction In(TimeSpan dueTime)
        {
            return new TimedAction(dueTime, TimeSpan.Zero, this);
        }

        public TimedAction Every(TimeSpan interval)
        {
            return new TimedAction(TimeSpan.FromMilliseconds(1), interval, this);
        }

        public void Run()
        {
            Log.Verbose($"Timer is running on thread {Environment.CurrentManagedThreadId}");

            while (true)
            {
                SpinWait.SpinUntil(() => _stopwatch.ElapsedMilliseconds >= 50);
                
                TimeSpan elapsedTime = _stopwatch.Elapsed;
                _stopwatch.Restart();

                InvokeTickEvent(elapsedTime);
            }
        }

        private void InvokeTickEvent(TimeSpan elapsedTime)
        {
            try
            {
                Tick?.Invoke(this, new TimerTickEventArgs(elapsedTime));
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Timer tick has catched an unhandled exception");
            }
        }
    }
}
