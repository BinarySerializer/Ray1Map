using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// Tile texture data for PC
    /// </summary>
    public class R1_PC_TileTextureBlock : BinarySerializable
    {
        /// <summary>
        /// The checksum for the decrypted texture block
        /// </summary>
        public byte TextureBlockChecksum { get; set; }

        /// <summary>
        /// The offset table for the <see cref="NonTransparentTextures"/> and <see cref="TransparentTextures"/>
        /// </summary>
        public Pointer[] TexturesOffsetTable { get; set; }

        /// <summary>
        /// The total amount of textures for <see cref="NonTransparentTextures"/> and <see cref="TransparentTextures"/>
        /// </summary>
        public uint TexturesCount { get; set; }

        /// <summary>
        /// The amount of <see cref="NonTransparentTextures"/>
        /// </summary>
        public uint NonTransparentTexturesCount { get; set; }

        /// <summary>
        /// The byte size of <see cref="NonTransparentTextures"/>, <see cref="TransparentTextures"/> and <see cref="Unknown4"/>
        /// </summary>
        public uint TexturesDataTableCount { get; set; }

        /// <summary>
        /// The textures which are not transparent
        /// </summary>
        public R1_PC_TileTexture[] NonTransparentTextures { get; set; }

        /// <summary>
        /// The textures which have transparency
        /// </summary>
        public R1_PC_TransparentTileTexture[] TransparentTextures { get; set; }

        /// <summary>
        /// Unknown array of bytes, always 32 in length
        /// </summary>
        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// The checksum for <see cref="NonTransparentTextures"/>, <see cref="TransparentTextures"/> and <see cref="Unknown4"/>
        /// </summary>
        public byte TexturesChecksum { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            TextureBlockChecksum = s.DoChecksum(new Checksum8Calculator(false), () =>
            {
                s.DoXOR((byte)(s.GetR1Settings().EngineVersion == EngineVersion.R1_PC || s.GetR1Settings().EngineVersion == EngineVersion.R1_PocketPC ? 0 : 0xFF), () =>
                {
                    // Read the offset table for the textures, based from the start of the tile texture arrays
                    TexturesOffsetTable = s.SerializePointerArray(TexturesOffsetTable, 1200, s.CurrentPointer + 1200 * 4 + 3 * 4, name: nameof(TexturesOffsetTable));

                    // Read the textures count
                    TexturesCount = s.Serialize<uint>(TexturesCount, name: nameof(TexturesCount));
                    NonTransparentTexturesCount = s.Serialize<uint>(NonTransparentTexturesCount, name: nameof(NonTransparentTexturesCount));
                    TexturesDataTableCount = s.Serialize<uint>(TexturesDataTableCount, name: nameof(TexturesDataTableCount));
                });

                TexturesChecksum = s.DoChecksum(new Checksum8Calculator(), () =>
                {
                    // Serialize the textures
                    NonTransparentTextures = s.SerializeObjectArray<R1_PC_TileTexture>(NonTransparentTextures, NonTransparentTexturesCount, name: nameof(NonTransparentTextures));
                    TransparentTextures = s.SerializeObjectArray<R1_PC_TransparentTileTexture>(TransparentTextures, TexturesCount - NonTransparentTexturesCount, name: nameof(TransparentTextures));

                    // Serialize the fourth unknown value
                    Unknown4 = s.SerializeArray<byte>(Unknown4, 32, name: nameof(Unknown4));
                }, ChecksumPlacement.After, calculateChecksum: s.GetR1Settings().EngineVersion == EngineVersion.R1_PC || s.GetR1Settings().EngineVersion == EngineVersion.R1_PocketPC, name: nameof(TexturesChecksum));
            }, ChecksumPlacement.Before, calculateChecksum: s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Kit || s.GetR1Settings().EngineVersion == EngineVersion.R1_PC_Edu, name: nameof(TextureBlockChecksum));
        }
    }
}