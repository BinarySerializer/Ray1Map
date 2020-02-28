using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Map tile data for Rayman 1 (PC)
    /// </summary>
    public class PC_R1_MapTile : ISerializableFile
    {
        /// <summary>
        /// The index for the texture for this cell
        /// </summary>
        public ushort TextureIndex { get; set; }

        /// <summary>
        /// The collision type
        /// </summary>
        public TileCollisionType CollisionType { get; set; }

        /// <summary>
        /// An unknown byte
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// The transparency mode for this cell
        /// </summary>
        public PC_R1_MapTileTransparencyMode TransparencyMode { get; set; }

        /// <summary>
        /// An unknown byte
        /// </summary>
        public byte Unknown2 { get; set; }

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            TextureIndex = stream.Read<ushort>();
            CollisionType = (TileCollisionType)stream.Read<byte>();
            Unknown1 = stream.Read<byte>();
            TransparencyMode = (PC_R1_MapTileTransparencyMode)stream.Read<byte>();
            Unknown2 = stream.Read<byte>();
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            stream.Write(TextureIndex);
            stream.Write((byte)CollisionType);
            stream.Write(Unknown1);
            stream.Write((byte)TransparencyMode);
            stream.Write(Unknown2);
        }

        /// <summary>
        /// The transparency mode for a <see cref="PC_R1_MapTile"/>
        /// </summary>
        public enum PC_R1_MapTileTransparencyMode
        {
            /// <summary>
            /// Indicates that the cell has no transparency
            /// </summary>
            NoTransparency = 0,

            /// <summary>
            /// Indicates that the cell is fully transparent
            /// </summary>
            FullyTransparent = 1,

            /// <summary>
            /// Indicates that the cell is partially transparent
            /// </summary>
            PartiallyTransparent = 2
        }
    }
}