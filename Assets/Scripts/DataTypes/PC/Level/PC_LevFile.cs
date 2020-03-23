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
            EventBlockPointer = s.Serialize(EventBlockPointer, name: "EventBlockPointer");
            TextureBlockPointer = s.Serialize(TextureBlockPointer, name: "TextureBlockPointer");

            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
                Unknown6 = s.SerializeArray(Unknown6, 68, name: "Unknown6");

            // Serialize map size
            Width = s.Serialize(Width, name: "Width");
            Height = s.Serialize(Height, name: "Height");

            // Create the palettes if necessary
            if (ColorPalettes == null) {
                ColorPalettes = s.GameSettings.GameMode == GameMode.RayKit ? new RGB666Color[][]
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
                ColorPalettes[paletteIndex] = s.SerializeObjectArray(palette, palette.Length, name: "ColorPalettes[" + paletteIndex + "]");
            }

            // Serialize unknown byte
            LastPlan1Palette = s.Serialize(LastPlan1Palette, name: "LastPlan1Palette");

            // MAP BLOCK

            // Serialize the map cells
            Tiles = s.SerializeObjectArray(Tiles, Height * Width, name: "Tiles");

            if (s.GameSettings.GameMode == GameMode.RayPC || s.GameSettings.GameMode == GameMode.RayPocketPC)
            {
                // Serialize the background data
                BackgroundIndex = s.Serialize(BackgroundIndex, name: "BackgroundIndex");
                ParallaxBackgroundIndex = s.Serialize(ParallaxBackgroundIndex, name: "ParallaxBackgroundIndex");
                BackgroundSpritesDES = s.Serialize(BackgroundSpritesDES, name: "BackgroundSpritesDES");

                if (s.GameSettings.GameMode == GameMode.RayPC)
                {
                    // Serialize the rough textures count
                    RoughTextureCount = s.Serialize(RoughTextureCount, name: "RoughTextureCount");

                    // Serialize the length of the third unknown value
                    Unknown3Count = s.Serialize(Unknown3Count, name: "Unknown3Count");

                    // Begin calculating the rough texture checksum
                    s.BeginCalculateChecksum(new Checksum8Calculator());

                    // TODO: Encrypted with xor 0xFD
                    // Create the collection of rough textures if necessary
                    if (RoughTextures == null) {
                        RoughTextures = new byte[RoughTextureCount][];
                    }

                    // Serialize each rough texture
                    for (int i = 0; i < RoughTextureCount; i++)
                        RoughTextures[i] = s.SerializeArray(RoughTextures[i], PC_Manager.CellSize * PC_Manager.CellSize, "RoughTextures[" + i + "]");

                    // Get the checksum
                    var c1 = s.EndCalculateChecksum<byte>();

                    // Read & verify the checksum for the rough textures
                    RoughTexturesChecksum = s.SerializeChecksum(c1, name: "RoughTexturesChecksum");

                    // Read the index table for the rough textures
                    RoughTexturesIndexTable = s.SerializeArray(RoughTexturesIndexTable, 1200, name: "RoughTexturesIndexTable");

                    // Begin calculating the unknown 3 checksum
                    s.BeginCalculateChecksum(new Checksum8Calculator());

                    // TODO: Encrypted with xor 0xF3
                    // Serialize the items for the third unknown value
                    Unknown3 = s.SerializeArray(Unknown3, Unknown3Count, name: "Unknown3");

                    // Get the checksum
                    var c2 = s.EndCalculateChecksum<byte>();

                    // Serialize the checksum for the third unknown value
                    Unknown3Checksum = s.SerializeChecksum(c2, name: "Unknown3Checksum");

                    // Read the offset table for the third unknown value
                    Unknown3OffsetTable = s.SerializeArray(Unknown3OffsetTable, 1200, name: "Unknown3OffsetTable");
                }
                else
                {
                    // Read unknown values
                    Unknown7 = s.SerializeArray(Unknown7, TextureBlockPointer - s.CurrentPointer.FileOffset, name: "Unknown7");
                }
            }
            else
            {
                // Read unknown values
                Unknown7 = s.SerializeArray(Unknown7, TextureBlockPointer - s.CurrentPointer.FileOffset, name: "Unknown7");
            }

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (s.CurrentPointer.FileOffset != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
                // TODO: Verify checksum
                TextureBlockChecksum = s.Serialize(TextureBlockChecksum, name: "TextureBlockChecksum");

            // Get the xor key to use for the texture block
            byte texXor = (byte)(s.GameSettings.GameMode == GameMode.RayPC || s.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 255);
            s.BeginXOR(texXor);

            // Read the offset table for the textures
            TexturesOffsetTable = s.SerializeArray(TexturesOffsetTable, 1200, name: "TexturesOffsetTable");

            // Read the textures count
            TexturesCount = s.Serialize(TexturesCount, name: "TexturesCount");
            NonTransparentTexturesCount = s.Serialize(NonTransparentTexturesCount, name: "NonTransparentTexturesCount");
            TexturesDataTableCount = s.Serialize(TexturesDataTableCount, name: "TexturesDataTableCount");
            s.EndXOR();

            // Get the current offset to use for the texture offsets
            var textureBaseOffset = s.CurrentPointer.FileOffset;

            // Begin calculating the texture checksum
            if (s.GameSettings.GameMode == GameMode.RayPC || s.GameSettings.GameMode == GameMode.RayPocketPC)
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
                NonTransparentTextures[i] = s.SerializeObject(NonTransparentTextures[i], onPreSerialize: t => t.TextureOffset = texOffset, name: "NonTransparentTextures[" + i + "]");
            }

            if (TransparentTextures == null) {
                // Create the collection of transparent textures
                TransparentTextures = new PC_TransparentTileTexture[TexturesCount - NonTransparentTexturesCount];
            }

            // Read the transparent textures
            for (int i = 0; i < TransparentTextures.Length; i++)
            {
                uint texOffset = s.CurrentPointer.FileOffset - textureBaseOffset;
                TransparentTextures[i] = s.SerializeObject(TransparentTextures[i], onPreSerialize: t => t.TextureOffset = texOffset, name: "TransparentTextures[" + i + "]");
            }

            // Serialize the fourth unknown value
            Unknown4 = s.SerializeArray(Unknown4, 32, name: "Unknown4");

            if (s.GameSettings.GameMode == GameMode.RayPC || s.GameSettings.GameMode == GameMode.RayPocketPC)
            {
                // Get the checksum
                var c = s.EndCalculateChecksum<byte>();

                // Serialize the checksum for the textures
                TexturesChecksum = s.SerializeChecksum(c, name: "TexturesChecksum");
            }

            // EVENT BLOCK

            // At this point the stream position should match the event block offset (ignore the Pocket PC version here since it uses leftover pointers from PC version)
            if (s.GameSettings.GameMode != GameMode.RayPocketPC && s.CurrentPointer.FileOffset != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            if (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC)
                // TODO: Verify checksum
                EventBlockChecksum = s.Serialize(EventBlockChecksum, name: "EventBlockChecksum");

            // Set the xor key to use for the event block
            s.BeginXOR((byte)(s.GameSettings.GameMode == GameMode.RayPC || s.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 145));

            // Serialize the event count
            EventCount = s.Serialize(EventCount, name: "EventCount");

            // Serialize the event linking table
            EventLinkingTable = s.SerializeArray(EventLinkingTable, EventCount, name: "EventLinkingTable");

            // Serialize the events
            Events = s.SerializeObjectArray(Events, EventCount, name: "Events");

            // Serialize the event commands
            EventCommands = s.SerializeObjectArray(EventCommands, EventCount, name: "EventCommands");

            s.EndXOR();

            // Serialize remaining data (appears in some Kit levels)
            Unknown8 = s.SerializeArray(Unknown8, s.CurrentLength - s.CurrentPointer.FileOffset, name: "Unknown8");
        }

        #endregion
    }
}