namespace R1Engine
{
    /// <summary>
    /// Common event data
    /// </summary>
    public class Common_EventData
    {
        /// <summary>
        /// The event type
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// The event state
        /// </summary>
        public int Etat { get; set; }

        /// <summary>
        /// The event sub-state
        /// </summary>
        public int SubEtat { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public uint XPosition { get; set; }

        /// <summary>
        /// The x position
        /// </summary>
        public uint YPosition { get; set; }

        /// <summary>
        /// The event design index
        /// </summary>
        public int DES { get; set; }

        /// <summary>
        /// The event ETA index
        /// </summary>
        public int ETA { get; set; }

        /// <summary>
        /// The event offset BX
        /// </summary>
        public int OffsetBX { get; set; }

        /// <summary>
        /// The event offset BY
        /// </summary>
        public int OffsetBY { get; set; }

        /// <summary>
        /// The event offset HY
        /// </summary>
        public int OffsetHY { get; set; }

        public int FollowSprite { get; set; }

        /// <summary>
        /// The event hit-points
        /// </summary>
        public int HitPoints { get; set; }

        /// <summary>
        /// The event layer
        /// </summary>
        public int Layer { get; set; }

        public int HitSprite { get; set; }

        /// <summary>
        /// Indicates if the event has collision
        /// </summary>
        public bool FollowEnabled { get; set; }

        /// <summary>
        /// The label offsets
        /// </summary>
        public ushort[] LabelOffsets { get; set; }

        /// <summary>
        /// The event commands
        /// </summary>
        public Common_EventCommandCollection CommandCollection { get; set; }

        /// <summary>
        /// The link table index
        /// </summary>
        public int LinkIndex { get; set; }
    }
}