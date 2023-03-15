namespace DDS.Net.Connector.Types
{
    public enum Periodicity
    {
        /// <summary>
        /// Updates only when changed.
        /// </summary>
        OnChange = 0,
        /// <summary>
        /// Updates every <c cref="Settings.BASE_TIME_SLOT_MS">BASE_TIME_SLOT_MS * 1</c> milliseconds.
        /// </summary>
        Highest,
        /// <summary>
        /// Updates every <c cref="Settings.BASE_TIME_SLOT_MS">BASE_TIME_SLOT_MS * 2</c> milliseconds.
        /// </summary>
        High,
        /// <summary>
        /// Updates every <c cref="Settings.BASE_TIME_SLOT_MS">BASE_TIME_SLOT_MS * 4</c> milliseconds.
        /// </summary>
        Normal,
        /// <summary>
        /// Updates every <c cref="Settings.BASE_TIME_SLOT_MS">BASE_TIME_SLOT_MS * 8</c> milliseconds.
        /// </summary>
        Low,
        /// <summary>
        /// Updates every <c cref="Settings.BASE_TIME_SLOT_MS">BASE_TIME_SLOT_MS * 16</c> milliseconds.
        /// </summary>
        Lowest
    }
}
