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
        private readonly bool isPeriodicCall;   // True means to run the action at intervals
        private readonly Action threadFunction; // Function to be invoked
        private readonly int timeInterval;

        public EasyThread(Action threadFunction)
        {
            this.threadFunction = threadFunction ?? throw new ArgumentNullException(nameof(threadFunction));
            isPeriodicCall = false;
        }

        public EasyThread(Action threadFunction, int timeInterval)
        {
            this.threadFunction = threadFunction ?? throw new ArgumentNullException(nameof(threadFunction));
            this.timeInterval = timeInterval;

            if (timeInterval < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeInterval));
            }

            isPeriodicCall = true;
        }
    }
}
