using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDS.Net.Connector.Helpers
{
    /// <summary>
    /// Class <c>EasyThread</c> helps in management of thread.
    /// </summary>
    internal class EasyThread
    {
        private readonly bool isPeriodic;                   // True means to run the action at intervals
        private readonly Func<bool> threadFunction = null!; // Function to be invoked continuously

        private readonly Action periodicFunction = null!;   // Function to be invoked periodically
        private readonly int timeInterval;                  // Periodic invocation time interval

        /// <summary>
        /// Initializes the instance with continuously invoked thread function.
        /// </summary>
        /// <param name="threadFunction">Target function - returns [True/False] if work is done during the iteration.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public EasyThread(Func<bool> threadFunction)
        {
            isPeriodic = false;

            this.threadFunction = threadFunction ?? throw new ArgumentNullException(nameof(threadFunction));
        }
        /// <summary>
        /// Initializes the instance with periodically invoked thread function.
        /// </summary>
        /// <param name="periodicFunction">Periodically invoked function.</param>
        /// <param name="timeInterval">Time period (milliseconds) between invokations - must be greater than zero.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public EasyThread(Action periodicFunction, int timeInterval)
        {
            isPeriodic = true;

            this.periodicFunction = periodicFunction ?? throw new ArgumentNullException(nameof(periodicFunction));
            this.timeInterval = timeInterval;

            if (timeInterval < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeInterval));
            }
        }

        private volatile bool isRunning = false;

        public void Start()
        {
            lock (this)
            {
                if (!isRunning)
                {
                    isRunning = true;
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
                }
            }
        }
    }
}
