using System.ComponentModel;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Level data for PC
    /// </summary>
    [Description("Rayman PC Level File")]
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
        public ARGBColor[][] ColorPalettes { get; set; }

        /// <summary>
        /// Unknown byte, always set to 2
        /// </summary>
        public byte Unknown10 { get; set; }

        /// <summary>
        /// The tiles for the map
        /// </summary>
        public PC_MapTile[] Tiles { get; set; }

        // TODO: This probably contains the same data as the below properties for Kit and Edu, but encrypted?
        public byte[] Unknown7 { get; set; }

        /// <summary>
        /// Unknown byte, different for each level
        /// </summary>
        public byte Unknown2 { get; set; }

        /// <summary>
        /// The index of the background image
        /// </summary>
        public byte BackgroundIndex { get; set; }

        /// <summary>
        /// The DES for the background sprites
        /// </summary>
        public uint BackgroundSpritesDES { get; set; }

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
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            // PC HEADER

            base.Serialize(serializer);

            // HEADER BLOCK

            // Serialize block pointer
            serializer.Serialize(nameof(EventBlockPointer));
            serializer.Serialize(nameof(TextureBlockPointer));

            if (serializer.GameSettings.GameMode == GameMode.RayKit || serializer.GameSettings.GameMode == GameMode.RayEduPC)
                serializer.SerializeArray<byte>(nameof(Unknown6), 68);

            // Serialize map size
            serializer.Serialize(nameof(Width));
            serializer.Serialize(nameof(Height));

            // Serialize the palettes
            if (serializer.Mode == SerializerMode.Read)
            {
                // Create the palettes
                ColorPalettes = serializer.GameSettings.GameMode == GameMode.RayKit ? new ARGBColor[][]
                {
                    new ARGBColor[256],
                } : new ARGBColor[][]
                {
                    new ARGBColor[256],
                    new ARGBColor[256],
                    new ARGBColor[256],
                };

                // Read each palette color
                for (var paletteIndex = 0; paletteIndex < ColorPalettes.Length; paletteIndex++)
                {
                    // Get the palette
                    var palette = ColorPalettes[paletteIndex];

                    // Read each color
                    for (int i = 0; i < palette.Length; i++)
                    {
                        // Read the palette color as RGB and multiply by 4 (as the values are between 0-64)
                        palette[i] = new ARGBColor((byte)(serializer.Read<byte>() * 4), (byte)(serializer.Read<byte>() * 4),
                            (byte)(serializer.Read<byte>() * 4));
                    }

                    // Reverse the palette
                    ColorPalettes[paletteIndex] = palette;
                }
            }
            else
            {
                // Write each palette
                foreach (var palette in ColorPalettes)
                {
                    foreach (var color in palette)
                    {
                        // Write the palette color as RGB and divide by 4 (as the values are between 0-64)
                        serializer.Write((byte)(color.Red / 4));
                        serializer.Write((byte)(color.Green / 4));
                        serializer.Write((byte)(color.Blue / 4));
                    }
                }
            }

            // Serialize unknown byte
            serializer.Serialize(nameof(Unknown10));

            // MAP BLOCK

            // Serialize the map cells
            serializer.SerializeArray<PC_MapTile>(nameof(Tiles), Height * Width);

            if (serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC)
            {
                // Serialize unknown byte
                serializer.Serialize(nameof(Unknown2));

                // Serialize the background data
                serializer.Serialize(nameof(BackgroundIndex));
                serializer.Serialize(nameof(BackgroundSpritesDES));

                if (serializer.GameSettings.GameMode == GameMode.RayPC)
                {
                    // Serialize the rough textures count
                    serializer.Serialize(nameof(RoughTextureCount));

                    // Serialize the length of the third unknown value
                    serializer.Serialize(nameof(Unknown3Count));

                    // Begin calculating the rough texture checksum
                    serializer.BeginCalculateChecksum(new Checksum8Calculator());

                    if (serializer.Mode == SerializerMode.Read)
                    {
                        // Create the collection of rough textures
                        RoughTextures = new byte[RoughTextureCount][];

                        // Read each rough texture
                        for (int i = 0; i < RoughTextureCount; i++)
                            RoughTextures[i] = serializer.ReadArray<byte>(PC_Manager.CellSize * PC_Manager.CellSize);
                    }
                    else
                    {
                        // Write each rough texture
                        for (int i = 0; i < RoughTextureCount; i++)
                            serializer.Write(RoughTextures[i]);
                    }

                    // Get the checksum
                    var c1 = serializer.EndCalculateChecksum<byte>();

                    // Read the checksum for the rough textures
                    serializer.Serialize(nameof(RoughTexturesChecksum));

                    // Verify the checksum
                    if (c1 != RoughTexturesChecksum && serializer.Mode == SerializerMode.Read)
                        Debug.LogWarning("Rough textures checksum does not match");

                    // Read the index table for the rough textures
                    serializer.SerializeArray<uint>(nameof(RoughTexturesIndexTable), 1200);

                    // Begin calculating the unknown 3 checksum
                    serializer.BeginCalculateChecksum(new Checksum8Calculator());

                    // Serialize the items for the third unknown value
                    serializer.SerializeArray<byte>(nameof(Unknown3), Unknown3Count);

                    // Get the checksum
                    var c2 = serializer.EndCalculateChecksum<byte>();

                    // Serialize the checksum for the third unknown value
                    serializer.Serialize(nameof(Unknown3Checksum));

                    // Verify the checksum
                    if (c2 != Unknown3Checksum && serializer.Mode == SerializerMode.Read)
                        Debug.LogWarning("Unknown 3 checksum does not match");

                    // Read the offset table for the third unknown value
                    serializer.SerializeArray<uint>(nameof(Unknown3OffsetTable), 1200);
                }
                else
                {
                    // Read unknown values
                    serializer.SerializeArray<byte>(nameof(Unknown7), TextureBlockPointer - serializer.BaseStream.Position);
                }
            }
            else
            {
                // Read unknown values
                serializer.SerializeArray<byte>(nameof(Unknown7), TextureBlockPointer - serializer.BaseStream.Position);
            }

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (serializer.BaseStream.Position != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            if (serializer.GameSettings.GameMode == GameMode.RayKit || serializer.GameSettings.GameMode == GameMode.RayEduPC)
                // TODO: Verify checksum
                serializer.Serialize(nameof(TextureBlockChecksum));

            // Get the xor key to use for the texture block
            byte texXor = (byte)(serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 255);

            // Read the offset table for the textures
            serializer.SerializeArray<uint>(nameof(TexturesOffsetTable), 1200, texXor);

            // Read the textures count
            serializer.Serialize(nameof(TexturesCount), texXor);
            serializer.Serialize(nameof(NonTransparentTexturesCount), texXor);
            serializer.Serialize(nameof(TexturesDataTableCount), texXor);

            // Get the current offset to use for the texture offsets
            var textureBaseOffset = serializer.BaseStream.Position;

            // Begin calculating the texture checksum
            if (serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC)
                serializer.BeginCalculateChecksum(new Checksum8Calculator());

            if (serializer.Mode == SerializerMode.Read)
            {
                // Create the collection of non-transparent textures
                NonTransparentTextures = new PC_TileTexture[NonTransparentTexturesCount];

                // Read the non-transparent textures
                for (int i = 0; i < NonTransparentTextures.Length; i++)
                {
                    // Create the texture
                    var t = new PC_TileTexture()
                    {
                        // Set the offset
                        Offset = (uint)(serializer.BaseStream.Position - textureBaseOffset)
                    };

                    // Deserialize the texture
                    t.Serialize(new BinarySerializer(serializer.Mode, serializer.BaseStream, t, serializer.FilePath, serializer.GameSettings));

                    // Add the texture to the collection
                    NonTransparentTextures[i] = t;
                }

                // Create the collection of transparent textures
                TransparentTextures = new PC_TransparentTileTexture[TexturesCount - NonTransparentTexturesCount];

                // Read the transparent textures
                for (int i = 0; i < TransparentTextures.Length; i++)
                {
                    // Create the texture
                    var t = new PC_TransparentTileTexture()
                    {
                        // Set the offset
                        Offset = (uint)(serializer.BaseStream.Position - textureBaseOffset)
                    };

                    // Deserialize the texture
                    t.Serialize(new BinarySerializer(serializer.Mode, serializer.BaseStream, t, serializer.FilePath, serializer.GameSettings));

                    // Add the texture to the collection
                    TransparentTextures[i] = t;
                }
            }
            else
            {
                // Write the non-transparent textures
                foreach (var texture in NonTransparentTextures)
                    // Write the texture
                    serializer.Write(texture);

                // Write the transparent textures
                foreach (var texture in TransparentTextures)
                    // Write the texture
                    serializer.Write(texture);
            }

            // Serialize the fourth unknown value
            serializer.SerializeArray<byte>(nameof(Unknown4), 32);

            if (serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC)
            {
                // Get the checksum
                var c = serializer.EndCalculateChecksum<byte>();

                // Serialize the checksum for the textures
                serializer.Serialize(nameof(TexturesChecksum));

                // Verify the checksum
                if (c != TexturesChecksum && serializer.Mode == SerializerMode.Read)
                    Debug.LogWarning("Texture checksum does not match");
            }

            // EVENT BLOCK

            // At this point the stream position should match the event block offset (ignore the Pocket PC version here since it uses leftover pointers from PC version)
            if (serializer.GameSettings.GameMode != GameMode.RayPocketPC && serializer.BaseStream.Position != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            if (serializer.GameSettings.GameMode == GameMode.RayKit || serializer.GameSettings.GameMode == GameMode.RayEduPC)
                // TODO: Verify checksum
                serializer.Serialize(nameof(EventBlockChecksum));

            // Get the xor key to use for the event block
            byte eveXor = (byte)(serializer.GameSettings.GameMode == GameMode.RayPC || serializer.GameSettings.GameMode == GameMode.RayPocketPC ? 0 : 145);

            // Serialize the event count
            serializer.Serialize(nameof(EventCount), eveXor);

            // Serialize the event linking table
            serializer.SerializeArray<ushort>(nameof(EventLinkingTable), EventCount, eveXor);

            // Serialize the events
            serializer.SerializeArray<PC_Event>(nameof(Events), EventCount, eveXor);

            // Serialize the event commands
            serializer.SerializeArray<PC_EventCommand>(nameof(EventCommands), EventCount, eveXor);

            // Serialize remaining data (appears in some Kit levels)
            serializer.SerializeArray<byte>(nameof(Unknown8), serializer.BaseStream.Length - serializer.BaseStream.Position);
        }

        #endregion
    }
}