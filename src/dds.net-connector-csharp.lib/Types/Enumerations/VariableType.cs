namespace DDS.Net.Connector.Types.Enumerations
{
    /// <summary>
    /// Denotes a major data type.
    /// Add more major data types before the last (<c>UnknownVariableType</c>) one.
    /// </summary>
    internal enum VariableType
    {
        /// <summary>
        /// Represents the very basic variable type,
        /// e.g., string, integer, float, etc.
        /// </summary>
        Primitive,
        /// <summary>
        /// Represents a sequence of bytes (unsigned bytes).
        /// </summary>
        RawBytes,

        /// <summary>
        /// Unknown type.
        /// </summary>
        UnknownVariableType
    }
}
