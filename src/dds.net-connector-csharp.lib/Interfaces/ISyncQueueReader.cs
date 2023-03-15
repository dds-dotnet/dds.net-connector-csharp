﻿namespace DDS.Net.Connector.Interfaces
{
    internal interface ISyncQueueReader<T>
    {
        /// <summary>
        /// Checks if the queue has any element to dequeue.
        /// </summary>
        /// <returns>True if any element is available, False otherwise.</returns>
        bool CanDequeue();
        /// <summary>
        /// Removes first available data element from the queue and returns it.
        /// Blocks if no data element is available, as long as any data element is made available.
        /// </summary>
        /// <returns>The data element.</returns>
        T Dequeue();
    }
}
