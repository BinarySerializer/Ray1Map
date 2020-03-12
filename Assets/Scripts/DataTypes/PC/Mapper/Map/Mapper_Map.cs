using System;

namespace R1Engine
{
    /// <summary>
    /// Map data for the Mapper
    /// </summary>
    public class Mapper_Map : IBinarySerializable
    {
        #region Public Properties

        /// <summary>
        /// The width of the map, in cells
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The height of the map, in cells
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The tiles for the map
        /// </summary>
        public Mapper_MapTile[] Tiles { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public void Deserialize(BinaryDeserializer deserializer)
        {
            // Read map size
            Width = deserializer.Read<ushort>();
            Height = deserializer.Read<ushort>();

            // Read tiles
            Tiles = deserializer.ReadArray<Mapper_MapTile>((ulong)(Width * Height));
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}