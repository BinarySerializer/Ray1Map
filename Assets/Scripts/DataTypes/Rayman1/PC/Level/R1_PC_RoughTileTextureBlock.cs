using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Rough tile texture data for PC
    /// </summary>
    public class R1_PC_RoughTileTextureBlock : BinarySerializable
    {
        /// <summary>
        /// The length of <see cref="GrosPataiBlock"/>
        /// </summary>
        public uint GrosPataiBlockCount { get; set; }

        /// <summary>
        /// The length of <see cref="BlocksCode"/>
        /// </summary>
        public uint BlocksCodeCount { get; set; }

        /// <summary>
        /// The color indexes for the rough textures
        /// </summary>
        public byte[][] GrosPataiBlock { get; set; }

        /// <summary>
        /// The checksum for the <see cref="GrosPataiBlock"/>
        /// </summary>
        public byte GrosPataiBlockChecksum { get; set; }

        /// <summary>
        /// The index table for the <see cref="GrosPataiBlock"/>
        /// </summary>
        public uint[] GrosPataiBlockOffsetTable { get; set; }

        /// <summary>
        /// Unknown array of bytes
        /// </summary>
        public byte[] BlocksCode { get; set; }

        /// <summary>
        /// The checksum for <see cref="BlocksCode"/>
        /// </summary>
        public byte BlocksCodeChecksum { get; set; }

        /// <summary>
        /// Offset table for <see cref="BlocksCode"/>
        /// </summary>
        public uint[] BlocksCodeOffsetTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // NOTE: This block is only parsed by the game if the rough textures should be used. Otherwise it skips to the texture block pointer.

            // Serialize the rough textures count
            GrosPataiBlockCount = s.Serialize<uint>(GrosPataiBlockCount, name: nameof(GrosPataiBlockCount));

            // Serialize the length of the third unknown value
            BlocksCodeCount = s.Serialize<uint>(BlocksCodeCount, name: nameof(BlocksCodeCount));

            GrosPataiBlockChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR(0x7D, () =>
                {
                    // Create the collection of rough textures if necessary
                    if (GrosPataiBlock == null)
                        GrosPataiBlock = new byte[GrosPataiBlockCount][];

                    // Serialize each rough texture
                    for (int i = 0; i < GrosPataiBlockCount; i++)
                        GrosPataiBlock[i] = s.SerializeArray<byte>(GrosPataiBlock[i], Settings.CellSize * Settings.CellSize, name:
                            $"{nameof(GrosPataiBlock)}[{i}]");
                });
            }, ChecksumPlacement.After, name: nameof(GrosPataiBlockChecksum));

            // Read the offset table for the rough textures
            GrosPataiBlockOffsetTable = s.SerializeArray<uint>(GrosPataiBlockOffsetTable, 1200, name: nameof(GrosPataiBlockOffsetTable));

            BlocksCodeChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                // Serialize the items for the third unknown value
                s.DoXOR(0xF3, () => BlocksCode = s.SerializeArray<byte>(BlocksCode, BlocksCodeCount, name: nameof(BlocksCode)));
            }, ChecksumPlacement.After, name: nameof(BlocksCodeChecksum));

            // Read the offset table for the third unknown value
            BlocksCodeOffsetTable = s.SerializeArray<uint>(BlocksCodeOffsetTable, 1200, name: nameof(BlocksCodeOffsetTable));
        }
    }
}