using System;

namespace R1Engine
{
    /// <summary>
    /// Attribute for event types
    /// </summary>
    public sealed class EventTypeInfoAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="flag">The event flag</param>
        public EventTypeInfoAttribute(EventFlag flag)
        {
            Flag = flag;
        }

        /// <summary>
        /// The event flag
        /// </summary>
        public EventFlag Flag { get; }
    }
}