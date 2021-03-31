using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Rough tile texture data for PC
    /// </summary>
    public class R1_PC_RoughTileTextureBlock : BinarySerializable
    {
        /// <summary>
        /// The length of <see cref="RoughTextures"/>
        /// </summary>
        public uint RoughTexturesCount { get; set; }

        /// <summary>
        /// The length of <see cref="Unknown3"/>
        /// </summary>
        public uint Unknown3Count { get; set; }

        /// <summary>
        /// The color indexes for the rough textures
        /// </summary>
        public byte[][] RoughTextures { get; set; }

        /// <summary>
        /// The checksum for the <see cref="RoughTextures"/>
        /// </summary>
        public byte RoughTexturesChecksum { get; set; }

        /// <summary>
        /// The index table for the <see cref="RoughTextures"/>
        /// </summary>
        public uint[] RoughTexturesOffsetTable { get; set; }

        /// <summary>
        /// Unknown array of bytes
        /// </summary>
        public byte[] Unknown3 { get; set; }

        /// <summary>
        /// The checksum for <see cref="Unknown3"/>
        /// </summary>
        public byte Unknown3Checksum { get; set; }

        /// <summary>
        /// Offset table for <see cref="Unknown3"/>
        /// </summary>
        public uint[] Unknown3OffsetTable { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // NOTE: This block is only parsed by the game if the rough textures should be used. Otherwise it skips to the texture block pointer.

            // Serialize the rough textures count
            RoughTexturesCount = s.Serialize<uint>(RoughTexturesCount, name: nameof(RoughTexturesCount));

            // Serialize the length of the third unknown value
            Unknown3Count = s.Serialize<uint>(Unknown3Count, name: nameof(Unknown3Count));

            RoughTexturesChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR(0x7D, () =>
                {
                    // Create the collection of rough textures if necessary
                    if (RoughTextures == null)
                        RoughTextures = new byte[RoughTexturesCount][];

                    // Serialize each rough texture
                    for (int i = 0; i < RoughTexturesCount; i++)
                        RoughTextures[i] = s.SerializeArray<byte>(RoughTextures[i], Settings.CellSize * Settings.CellSize, name:
                            $"{nameof(RoughTextures)}[{i}]");
                });
            }, ChecksumPlacement.After, name: nameof(RoughTexturesChecksum));

            // Read the offset table for the rough textures
            RoughTexturesOffsetTable = s.SerializeArray<uint>(RoughTexturesOffsetTable, 1200, name: nameof(RoughTexturesOffsetTable));

            Unknown3Checksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                // Serialize the items for the third unknown value
                s.DoXOR(0xF3, () => Unknown3 = s.SerializeArray<byte>(Unknown3, Unknown3Count, name: nameof(Unknown3)));
            }, ChecksumPlacement.After, name: nameof(Unknown3Checksum));

            // Read the offset table for the third unknown value
            Unknown3OffsetTable = s.SerializeArray<uint>(Unknown3OffsetTable, 1200, name: nameof(Unknown3OffsetTable));
        }
    }
}