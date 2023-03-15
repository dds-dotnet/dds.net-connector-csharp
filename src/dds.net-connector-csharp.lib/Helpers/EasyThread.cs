using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DDS.Net.Connector.Helpers
{
    /// <summary>
    /// Class <c>EasyThread</c> helps in management of thread.
    /// </summary>
    internal class EasyThread<T>
    {
        private readonly bool isPeriodic;                      // True means to run the action at intervals

        private readonly T data;                               // Data to be passed-in to the function

        private readonly Func<T, bool> threadFunction = null!; // Function to be invoked continuously
        private readonly int sleepTimeWhenDoneNothing;         // Sleep for milliseconds when the function did nothing

        private readonly Action<T> periodicFunction = null!;   // Function to be invoked periodically
        private readonly int timeInterval;                     // Periodic invocation time interval

        /// <summary>
        /// Initializes the instance with continuously invoked thread function.
        /// </summary>
        /// <param name="threadFunction">Target function - returns [True/False] if work is done during the iteration.</param>
        /// <param name="data">Data to be passed to the function.</param>
        /// <param name="sleepTimeWhenDoneNothing">Sleep time (milliseconds) when the thread function did nothing.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public EasyThread(Func<T, bool> threadFunction, T data, int sleepTimeWhenDoneNothing = 10)
        {
            isPeriodic = false;

            this.threadFunction = threadFunction ?? throw new ArgumentNullException(nameof(threadFunction));
            this.data = data;
            this.sleepTimeWhenDoneNothing = sleepTimeWhenDoneNothing;

            if (sleepTimeWhenDoneNothing <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sleepTimeWhenDoneNothing));
            }
        }
        /// <summary>
        /// Initializes the instance with periodically invoked thread function.
        /// </summary>
        /// <param name="periodicFunction">Periodically invoked function.</param>
        /// <param name="data">Data to be passed to the function.</param>
        /// <param name="timeInterval">Time period (milliseconds) between invokations - must be greater than zero.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public EasyThread(Action<T> periodicFunction, T data, int timeInterval)
        {
            isPeriodic = true;

            this.periodicFunction = periodicFunction ?? throw new ArgumentNullException(nameof(periodicFunction));
            this.data = data;
            this.timeInterval = timeInterval;

            if (timeInterval < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeInterval));
            }
        }

        private volatile bool isRunning = false;
        private Timer timer = null!;
        private Thread thread = null!;

        public void Start()
        {
            lock (this)
            {
                if (!isRunning)
                {
                    isRunning = true;

                    if (isPeriodic)
                    {
                        timer = new(TimerFunction, null, Timeout.Infinite, Timeout.Infinite);
                        timer.Change(TimeSpan.FromMilliseconds(timeInterval), Timeout.InfiniteTimeSpan);
                    }
                    else
                    {
                        thread = new(ThreadFunction);
                        thread.Start();
                    }
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (!isRunning)
                {
                    isRunning = false;

                    if (isPeriodic)
                    {
                        try
                        {
                            timer.Dispose();
                            timer = null!;
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            thread.Join(100);
                            thread = null!;
                        }
                        catch { }
                    }
                }
            }
        }

        private void TimerFunction(object? state)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);

            periodicFunction.Invoke(data);

            lock (this)
            {
                if (!isRunning)
                {
                    try
                    {
                        timer.Dispose();
                        timer = null!;
                    }
                    catch { }
                }
                else
                {
                    timer.Change(TimeSpan.FromMilliseconds(timeInterval), Timeout.InfiniteTimeSpan);
                }
            }
        }

        private void ThreadFunction(object? _)
        {
            while (isRunning)
            {
                bool doneAnything = threadFunction.Invoke(data);

                if (!doneAnything)
                {
                    Thread.Sleep(sleepTimeWhenDoneNothing);
                }
            }
        }
    }
}
