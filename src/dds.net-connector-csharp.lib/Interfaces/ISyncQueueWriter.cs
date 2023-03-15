namespace DDS.Net.Connector.Interfaces
{
    /// <summary>
    /// Interface <c>ISyncQueueWriter</c> represents a writer's end of a synchronized queue.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    internal interface ISyncQueueWriter<T>
    {
        /// <summary>
        /// Checks if the queue has enough space to enqueue any data element.
        /// </summary>
        /// <returns>True when the queue has empty space, False otherwise.</returns>
        bool CanEnqueue();
        /// <summary>
        /// Enqueues the provided data element into the queue.
        /// Blocks when the queue is not having any empty space, till the availability of space.
        /// </summary>
        /// <param name="data">The data element.</param>
        void Enqueue(T data);
    }
}
