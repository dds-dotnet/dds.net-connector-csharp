namespace DDS.Net.Connector.Interfaces
{
    /// <summary>
    /// Interface <c>ISyncQueueReader</c> represents a reader's end of a synchronized queue.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    internal interface ISyncQueueReader<T>
    {
        /// <summary>
        /// The event is initiated upon availability of data.
        /// </summary>
        event Action<T>? DataAvailable;
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
