namespace R1Engine
{
    /// <summary>
    /// Tile set data for Rayman 1 (PS1 - Japan)
    /// </summary>
    public class PS1_R1JP_TileSet : R1Serializable
    {
        /// <summary>
        /// The amount of tile color array
        /// </summary>
        public int TilesArrayLength { get; set; }

        /// <summary>
        /// The tile colors
        /// </summary>
        public RGB555Color[] Tiles { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize the array
            Tiles = s.SerializeObjectArray<RGB555Color>(Tiles, TilesArrayLength, name: nameof(Tiles));
        }
    }
}