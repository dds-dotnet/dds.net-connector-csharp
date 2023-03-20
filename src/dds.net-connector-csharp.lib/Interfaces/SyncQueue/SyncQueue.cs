namespace DDS.Net.Connector.Interfaces.SyncQueue
{
    /// <summary>
    /// Class <c>SyncQueue</c> implements both <c>ISyncQueueReader</c> and <c>ISyncQueueWriter</c>
    /// interfaces for reference types.
    /// </summary>
    /// <typeparam name="T">A reference type.</typeparam>
    internal class SyncQueue<T>
        : ISyncQueueWriter<T>, ISyncQueueReader<T>, IDisposable

        where T : class
    {
        private readonly int SLEEP_TIME_MS_WHEN_DATA_CANNOT_BE_DEQUEUED = 5;
        private readonly int SLEEP_TIME_MS_WHEN_DATA_CANNOT_BE_ENQUEUED = 5;

        private Mutex _mutex;

        private T[] _queue;
        private int _nextWriteIndex;
        private int _nextReadIndex;

        /// <summary>
        /// Initializes queue with specified size.
        /// </summary>
        /// <param name="queueSize">Total number of elements the queue can hold.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public SyncQueue(int queueSize = 100)
        {
            if (queueSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(queueSize));
            }

            _queue = new T[queueSize];

            for (int i = 0; i < queueSize; i++)
            {
                _queue[i] = null!;
            }

            _nextWriteIndex = 0;
            _nextReadIndex = 0;

            _mutex = new Mutex(false);
        }

        public bool CanDequeue()
        {
            lock (_mutex)
            {
                if (_queue[_nextReadIndex] != null)
                    return true;

                return false;
            }
        }

        public bool CanEnqueue()
        {
            lock (_mutex)
            {
                if (_queue[_nextWriteIndex] == null)
                    return true;

                return false;
            }
        }

        public T Dequeue()
        {
            while (true)
            {
                lock (_mutex)
                {
                    if (_queue[_nextReadIndex] != null)
                    {
                        T data = _queue[_nextReadIndex];
                        _queue[_nextReadIndex] = null!;

                        _nextReadIndex++;

                        if (_nextReadIndex == _queue.Length)
                            _nextReadIndex = 0;

                        return data;
                    }
                }

                Thread.Sleep(SLEEP_TIME_MS_WHEN_DATA_CANNOT_BE_DEQUEUED);
            }
        }

        public void Enqueue(T data)
        {
            if (data == null) return;

            while (true)
            {
                lock (_mutex)
                {
                    if (_queue[_nextWriteIndex] == null)
                    {
                        _queue[_nextWriteIndex] = data;

                        _nextWriteIndex++;

                        if (_nextWriteIndex == _queue.Length)
                            _nextWriteIndex = 0;

                        break;
                    }
                }

                Thread.Sleep(SLEEP_TIME_MS_WHEN_DATA_CANNOT_BE_ENQUEUED);
            }
        }

        public void Clear()
        {
            lock (_mutex)
            {
                for (int i = 0; i < _queue.Length; i++)
                {
                    _queue[i] = null!;
                }

                _nextWriteIndex = 0;
                _nextReadIndex = 0;
            }
        }

        public void Dispose()
        {

        }
    }
}
