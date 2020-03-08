using System;

namespace R1Engine
{
    /// <summary>
    /// An event ID
    /// </summary>
    public class EventID : IEquatable<EventID>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="type">The event type</param>
        /// <param name="etat">The event state</param>
        /// <param name="subEtat">The event sub-state</param>
        public EventID(int type, int etat, int subEtat)
        {
            Type = type;
            Etat = etat;
            SubEtat = subEtat;
        }

        /// <summary>
        /// The event type
        /// </summary>
        public int Type { get; }

        /// <summary>
        /// The event state
        /// </summary>
        public int Etat { get; }
        
        /// <summary>
        /// The event sub-state
        /// </summary>
        public int SubEtat { get; }

        #region Public Methods

        /// <summary>
        /// Checks if the other instance is equals to the current one
        /// </summary>
        /// <param name="other">The other instance to compare to the current one</param>
        /// <returns>True if the other instance is equals to the current one, false if not</returns>
        public bool Equals(EventID other) => Type == other?.Type && Etat == other?.Etat && SubEtat == other?.SubEtat;

        /// <summary>
        /// True if the specified object equals the current instance
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is EventID id && Equals(id);

        /// <summary>
        /// Gets the hash for the item
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Type;
                hashCode = (hashCode * 397) ^ Etat;
                hashCode = (hashCode * 397) ^ SubEtat;
                return hashCode;
            }
        }

        #endregion

        #region Static Operators

        /// <summary>
        /// Checks if the two IDs are the same
        /// </summary>
        /// <param name="a">The first ID</param>
        /// <param name="b">The second ID</param>
        /// <returns>True if they are the same, false if not</returns>
        public static bool operator ==(EventID a, EventID b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        /// <summary>
        /// Checks if the two IDs are not the same
        /// </summary>
        /// <param name="a">The first ID</param>
        /// <param name="b">The second ID</param>
        /// <returns>True if they are not the same, false if they are</returns>
        public static bool operator !=(EventID a, EventID b)
        {
            return !(a == b);
        }

        #endregion
    }
}