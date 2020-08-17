using System;

namespace R1Engine
{
    /// <summary>
    /// Attribute for event types
    /// </summary>
    public sealed class ObjTypeInfoAttribute : Attribute
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="flag">The event flag</param>
        public ObjTypeInfoAttribute(ObjTypeFlag flag)
        {
            Flag = flag;
        }

        /// <summary>
        /// The event flag
        /// </summary>
        public ObjTypeFlag Flag { get; }
    }
}