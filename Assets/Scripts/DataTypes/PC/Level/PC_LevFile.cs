using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Level data for PC
    /// </summary>
    public class PC_LevFile : PC_BaseFile
    {
        #region Public Properties

        /// <summary>
        /// The pointer to the event block
        /// </summary>
        public uint EventBlockPointer { get; set; }

        /// <summary>
        /// The pointer to the texture block
        /// </summary>
        public uint TextureBlockPointer { get; set; }

        // TODO: Does this contain the level name + description for Kit?
        public byte[] Unknown6 { get; set; }

        /// <summary>
        /// The width of the map, in cells
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The height of the map, in cells
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// The color palettes
        /// </summary>
        public RGB666Color[][] ColorPalettes { get; set; }

        /// <summary>
        /// Last Plan 1 Palettte, always set to 2
        /// </summary>
        public byte LastPlan1Palette { get; set; }

        /// <summary>
        /// The tiles for the map
        /// </summary>
        public PC_MapTile[] Tiles { get; set; }

        // TODO: This probably contains the same data as the below properties for Kit and Edu, but encrypted?
        public byte[] Unknown7 { get; set; }

        /// <summary>
        /// The index of the background image
        /// </summary>
        public byte BackgroundIndex { get; set; }

        /// <summary>
        /// The index of the parallax background image
        /// </summary>
        public byte ParallaxBackgroundIndex { get; set; }

        /// <summary>
        /// The DES for the background sprites when parallax scrolling is enabled
        /// </summary>
        public int BackgroundSpritesDES { get; set; }

        /// <summary>
        /// The length of <see cref="RoughTextures"/>
        /// </summary>
        public uint RoughTextureCount { get; set; }

        /// <summary>
        /// The length of <see cref="Unknown3"/>
        /// </summary>
        public uint Unknown3Count { get; set; }

        // WIP: Instead of int, each item is a texture with ONLY the ColorIndexes property
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
        public uint[] RoughTexturesIndexTable { get; set; }

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
        /// The checksum for the decrypted texture block
        /// </summary>
        public byte TextureBlockChecksum { get; set; }

        /// <summary>
        /// The offset table for the <see cref="NonTransparentTextures"/> and <see cref="TransparentTextures"/>
        /// </summary>
        public uint[] TexturesOffsetTable { get; set; }

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
        public PC_TileTexture[] NonTransparentTextures { get; set; }

        /// <summary>
        /// The textures which have transparency
        /// </summary>
        public PC_TransparentTileTexture[] TransparentTextures { get; set; }

        /// <summary>
        /// Unknown array of bytes, always 32 in length
        /// </summary>
        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// The checksum for <see cref="NonTransparentTextures"/>, <see cref="TransparentTextures"/> and <see cref="Unknown4"/>
        /// </summary>
        public byte TexturesChecksum { get; set; }

        /// <summary>
        /// The checksum for the decrypted event block
        /// </summary>
        public byte EventBlockChecksum { get; set; }

        /// <summary>
        /// The number of available events in the map
        /// </summary>
        public ushort EventCount { get; set; }

        /// <summary>
        /// Data table for event linking
        /// </summary>
        public ushort[] EventLinkingTable { get; set; }

        /// <summary>
        /// The events in the map
        /// </summary>
        public PC_Event[] Events { get; set; }

        /// <summary>
        /// The event commands in the map
        /// </summary>
        public PC_EventCommand[] EventCommands { get; set; }

        public byte[] Unknown8 { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            // PC HEADER
            base.SerializeImpl(s);

            // HEADER BLOCK

            // Serialize block pointer
            EventBlockPointer = s.Serialize<uint>(EventBlockPointer, name: nameof(EventBlockPointer));
            TextureBlockPointer = s.Serialize<uint>(TextureBlockPointer, name: nameof(TextureBlockPointer));

            if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC)
                Unknown6 = s.SerializeArray<byte>(Unknown6, 68, name: nameof(Unknown6));

            // Serialize map size
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            // Create the palettes if necessary
            if (ColorPalettes == null) {
                ColorPalettes = s.GameSettings.EngineVersion == EngineVersion.RayKitPC ? new RGB666Color[][]
                {
                    new RGB666Color[256],
                } : new RGB666Color[][]
                {
                    new RGB666Color[256],
                    new RGB666Color[256],
                    new RGB666Color[256],
                };
            }

            // Serialize each palette
            for (var paletteIndex = 0; paletteIndex < ColorPalettes.Length; paletteIndex++) {
                var palette = ColorPalettes[paletteIndex];
                ColorPalettes[paletteIndex] = s.SerializeObjectArray<RGB666Color>(palette, palette.Length, name: nameof(ColorPalettes) + "[" + paletteIndex + "]");
            }

            // Serialize unknown byte
            LastPlan1Palette = s.Serialize<byte>(LastPlan1Palette, name: nameof(LastPlan1Palette));

            // MAP BLOCK

            // Serialize the map cells
            Tiles = s.SerializeObjectArray<PC_MapTile>(Tiles, Height * Width, name: nameof(Tiles));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC)
            {
                // Serialize the background data
                BackgroundIndex = s.Serialize<byte>(BackgroundIndex, name: nameof(BackgroundIndex));
                ParallaxBackgroundIndex = s.Serialize<byte>(ParallaxBackgroundIndex, name: nameof(ParallaxBackgroundIndex));
                BackgroundSpritesDES = s.Serialize<int>(BackgroundSpritesDES, name: nameof(BackgroundSpritesDES));

                if (s.GameSettings.EngineVersion == EngineVersion.RayPC)
                {
                    // Serialize the rough textures count
                    RoughTextureCount = s.Serialize<uint>(RoughTextureCount, name: nameof(RoughTextureCount));

                    // Serialize the length of the third unknown value
                    Unknown3Count = s.Serialize<uint>(Unknown3Count, name: nameof(Unknown3Count));

                    // Begin calculating the rough texture checksum
                    s.BeginCalculateChecksum(new Checksum8Calculator());

                    // TODO: Encrypted with xor 0xFD
                    // Create the collection of rough textures if necessary
                    if (RoughTextures == null) {
                        RoughTextures = new byte[RoughTextureCount][];
                    }

                    // Serialize each rough texture
                    for (int i = 0; i < RoughTextureCount; i++)
                        RoughTextures[i] = s.SerializeArray<byte>(RoughTextures[i], PC_Manager.CellSize * PC_Manager.CellSize, "RoughTextures[" + i + "]");

                    // Get the checksum
                    var c1 = s.EndCalculateChecksum<byte>();

                    // Read & verify the checksum for the rough textures
                    RoughTexturesChecksum = s.SerializeChecksum<byte>(c1, name: nameof(RoughTexturesChecksum));

                    // Read the index table for the rough textures
                    RoughTexturesIndexTable = s.SerializeArray<uint>(RoughTexturesIndexTable, 1200, name: nameof(RoughTexturesIndexTable));

                    // Begin calculating the unknown 3 checksum
                    s.BeginCalculateChecksum(new Checksum8Calculator());

                    // TODO: Encrypted with xor 0xF3
                    // Serialize the items for the third unknown value
                    Unknown3 = s.SerializeArray<byte>(Unknown3, Unknown3Count, name: nameof(Unknown3));

                    // Get the checksum
                    var c2 = s.EndCalculateChecksum<byte>();

                    // Serialize the checksum for the third unknown value
                    Unknown3Checksum = s.SerializeChecksum<byte>(c2, name: nameof(Unknown3Checksum));

                    // Read the offset table for the third unknown value
                    Unknown3OffsetTable = s.SerializeArray<uint>(Unknown3OffsetTable, 1200, name: nameof(Unknown3OffsetTable));
                }
                else
                {
                    // Read unknown values
                    Unknown7 = s.SerializeArray<byte>(Unknown7, TextureBlockPointer - s.CurrentPointer.FileOffset, name: nameof(Unknown7));
                }
            }
            else
            {
                // Read unknown values
                Unknown7 = s.SerializeArray<byte>(Unknown7, TextureBlockPointer - s.CurrentPointer.FileOffset, name: nameof(Unknown7));
            }

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (s.CurrentPointer.FileOffset != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC)
                // TODO: Verify checksum
                TextureBlockChecksum = s.Serialize<byte>(TextureBlockChecksum, name: nameof(TextureBlockChecksum));

            // Get the xor key to use for the texture block
            byte texXor = (byte)(s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC ? 0 : 255);
            s.BeginXOR(texXor);

            // Read the offset table for the textures
            TexturesOffsetTable = s.SerializeArray<uint>(TexturesOffsetTable, 1200, name: nameof(TexturesOffsetTable));

            // Read the textures count
            TexturesCount = s.Serialize<uint>(TexturesCount, name: nameof(TexturesCount));
            NonTransparentTexturesCount = s.Serialize<uint>(NonTransparentTexturesCount, name: nameof(NonTransparentTexturesCount));
            TexturesDataTableCount = s.Serialize<uint>(TexturesDataTableCount, name: nameof(TexturesDataTableCount));
            s.EndXOR();

            // Get the current offset to use for the texture offsets
            var textureBaseOffset = s.CurrentPointer.FileOffset;

            // Begin calculating the texture checksum
            if (s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC)
                s.BeginCalculateChecksum(new Checksum8Calculator());

            if (NonTransparentTextures == null) {
                // Create the collection of non-transparent textures
                NonTransparentTextures = new PC_TileTexture[NonTransparentTexturesCount];
            }

            // Serialize the non-transparent textures
            for (int i = 0; i < NonTransparentTextures.Length; i++)
            {
                // Serialize the texture
                uint texOffset = s.CurrentPointer.FileOffset - textureBaseOffset;
                NonTransparentTextures[i] = s.SerializeObject<PC_TileTexture>(NonTransparentTextures[i], onPreSerialize: t => t.TextureOffset = texOffset, name: nameof(NonTransparentTextures) + "[" + i + "]");
            }

            if (TransparentTextures == null) {
                // Create the collection of transparent textures
                TransparentTextures = new PC_TransparentTileTexture[TexturesCount - NonTransparentTexturesCount];
            }

            // Read the transparent textures
            for (int i = 0; i < TransparentTextures.Length; i++)
            {
                uint texOffset = s.CurrentPointer.FileOffset - textureBaseOffset;
                TransparentTextures[i] = s.SerializeObject<PC_TransparentTileTexture>(TransparentTextures[i], onPreSerialize: t => t.TextureOffset = texOffset, name: nameof(TransparentTextures) + "[" + i + "]");
            }

            // Serialize the fourth unknown value
            Unknown4 = s.SerializeArray<byte>(Unknown4, 32, name: nameof(Unknown4));

            if (s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC)
            {
                // Get the checksum
                var c = s.EndCalculateChecksum<byte>();

                // Serialize the checksum for the textures
                TexturesChecksum = s.SerializeChecksum<byte>(c, name: nameof(TexturesChecksum));
            }

            // EVENT BLOCK

            // At this point the stream position should match the event block offset (ignore the Pocket PC version here since it uses leftover pointers from PC version)
            if (s.GameSettings.EngineVersion != EngineVersion.RayPocketPC && s.CurrentPointer.FileOffset != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            if (s.GameSettings.EngineVersion == EngineVersion.RayKitPC || s.GameSettings.EngineVersion == EngineVersion.RayEduPC)
                // TODO: Verify checksum
                EventBlockChecksum = s.Serialize<byte>(EventBlockChecksum, name: nameof(EventBlockChecksum));

            // Set the xor key to use for the event block
            s.BeginXOR((byte)(s.GameSettings.EngineVersion == EngineVersion.RayPC || s.GameSettings.EngineVersion == EngineVersion.RayPocketPC ? 0 : 145));

            // Serialize the event count
            EventCount = s.Serialize<ushort>(EventCount, name: nameof(EventCount));

            // Serialize the event linking table
            EventLinkingTable = s.SerializeArray<ushort>(EventLinkingTable, EventCount, name: nameof(EventLinkingTable));

            // Serialize the events
            Events = s.SerializeObjectArray<PC_Event>(Events, EventCount, name: nameof(Events));

            // Serialize the event commands
            EventCommands = s.SerializeObjectArray<PC_EventCommand>(EventCommands, EventCount, name: nameof(EventCommands));

            s.EndXOR();

            // Serialize remaining data (appears in some Kit levels)
            Unknown8 = s.SerializeArray<byte>(Unknown8, s.CurrentLength - s.CurrentPointer.FileOffset, name: nameof(Unknown8));
        }

        #endregion
    }
}