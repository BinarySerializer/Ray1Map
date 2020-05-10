namespace R1Engine
{
    /// <summary>
    /// Map data for Rayman Advance (GBA)
    /// </summary>
    public class GBA_R1_Map : R1Serializable
    {
        /// <summary>
        /// The map width, in tiles
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The map height, in tiles
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The tiles
        /// </summary>
        public GBA_R1_MapTile[] Tiles { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Tiles = s.SerializeObjectArray<GBA_R1_MapTile>(Tiles, Width * Height, name: nameof(Tiles));
        }
    }
}