using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace R1Engine
{
    /// <summary>
    /// Level data for Rayman 1 (PC)
    /// </summary>
    [Description("Rayman 1 (PC) Level File")]
    public class PC_R1_LevFile : ISerializableFile
    {
        #region Public Properties

        /// <summary>
        /// The pointer to the event block
        /// </summary>
        public uint EventBlockPointer { get; set; }

        /// <summary>
        /// The pointer to <see cref="TexturesOffsetTable"/>
        /// </summary>
        public uint TextureOffsetTablePointer { get; set; }

        /// <summary>
        /// The width of the map, in cells
        /// </summary>
        public ushort MapWidth { get; set; }

        /// <summary>
        /// The height of the map, in cells
        /// </summary>
        public ushort MapHeight { get; set; }

        /// <summary>
        /// The color palettes
        /// </summary>
        public ARGBColor[][] ColorPalettes { get; set; }

        /// <summary>
        /// Unknown byte, always set to 2
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// The tiles for the map
        /// </summary>
        public PC_R1_MapTile[] Tiles { get; set; }

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
        public PC_R1_TileTexture[] NonTransparentTextures { get; set; }

        /// <summary>
        /// The textures which have transparency
        /// </summary>
        public PC_R1_TransparentTileTexture[] TransparentTextures { get; set; }

        /// <summary>
        /// Unknown array of bytes, always 32 in length
        /// </summary>
        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// The checksum for <see cref="NonTransparentTextures"/>, <see cref="TransparentTextures"/> and <see cref="Unknown4"/>
        /// </summary>
        public byte TexturesChecksum { get; set; }

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
        public PC_R1_Event[] Events { get; set; }

        /// <summary>
        /// The event commands in the map
        /// </summary>
        public PC_R1_EventCommand[] EventCommands { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        public void Deserialize(Stream stream)
        {
            // HEADER BLOCK

            // Read block pointer
            EventBlockPointer = stream.Read<uint>();
            TextureOffsetTablePointer = stream.Read<uint>();

            // Read map size
            MapWidth = stream.Read<ushort>();
            MapHeight = stream.Read<ushort>();

            // Create the palettes
            ColorPalettes = new ARGBColor[][]
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
                    palette[i] = new ARGBColor((byte)(stream.Read<byte>() * 4), (byte)(stream.Read<byte>() * 4),
                        (byte)(stream.Read<byte>() * 4));
                }

                // Reverse the palette
                ColorPalettes[paletteIndex] = palette.Reverse().ToArray();
            }

            // Read unknown byte
            Unknown1 = stream.Read<byte>();

            // MAP BLOCK

            // Create the collection of map cells
            Tiles = new PC_R1_MapTile[MapWidth * MapHeight];

            // Read each map cell
            Tiles = stream.Read<PC_R1_MapTile>((ulong)MapHeight * MapWidth);

            // Read unknown byte
            Unknown2 = stream.Read<byte>();

            // Read the background data
            BackgroundIndex = stream.Read<byte>();
            BackgroundSpritesDES = stream.Read<uint>();

            // Read the rough textures count
            RoughTextureCount = stream.Read<uint>();

            // Read the length of the third unknown value
            Unknown3Count = stream.Read<uint>();

            // Create the collection of rough textures
            RoughTextures = new byte[RoughTextureCount][];

            // Read each rough texture
            for (int i = 0; i < RoughTextureCount; i++)
                RoughTextures[i] = stream.ReadBytes(PC_R1_Manager.CellSize * PC_R1_Manager.CellSize);

            // Read the checksum for the rough textures
            RoughTexturesChecksum = stream.Read<byte>();

            // Read the index table for the rough textures
            RoughTexturesIndexTable = stream.Read<uint>(1200);

            // Read the items for the third unknown value
            Unknown3 = stream.Read<byte>(Unknown3Count);

            // Read the checksum for the third unknown value
            Unknown3Checksum = stream.Read<byte>();

            // Read the offset table for the third unknown value
            Unknown3OffsetTable = stream.Read<uint>(1200);

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (stream.Position != TextureOffsetTablePointer)
                throw new Exception("Texture block offset is incorrect");

            // Read the offset table for the textures
            TexturesOffsetTable = stream.Read<uint>(1200);

            // Read the textures count
            TexturesCount = stream.Read<uint>();
            NonTransparentTexturesCount = stream.Read<uint>();
            TexturesDataTableCount = stream.Read<uint>();

            // Get the current offset to use for the texture offsets
            var textureBaseOffset = stream.Position;

            // Create the collection of non-transparent textures
            NonTransparentTextures = new PC_R1_TileTexture[NonTransparentTexturesCount];

            // Read the non-transparent textures
            for (int i = 0; i < NonTransparentTextures.Length; i++)
            {
                // Create the texture
                var t = new PC_R1_TileTexture()
                {
                    // Set the offset
                    Offset = (uint)(stream.Position - textureBaseOffset)
                };

                // Deserialize the texture
                t.Deserialize(stream);

                // Add the texture to the collection
                NonTransparentTextures[i] = t;
            }

            // Create the collection of transparent textures
            TransparentTextures = new PC_R1_TransparentTileTexture[TexturesCount - NonTransparentTexturesCount];

            // Read the transparent textures
            for (int i = 0; i < TransparentTextures.Length; i++)
            {
                // Create the texture
                var t = new PC_R1_TransparentTileTexture()
                {
                    // Set the offset
                    Offset = (uint)(stream.Position - textureBaseOffset)
                };

                // Deserialize the texture
                t.Deserialize(stream);

                // Add the texture to the collection
                TransparentTextures[i] = t;
            }

            // Read the fourth unknown value
            Unknown4 = stream.ReadBytes(32);

            // Read the checksum for the textures
            TexturesChecksum = stream.Read<byte>();

            // EVENT BLOCK

            // Read the event count
            EventCount = stream.Read<ushort>();

            // Read the event linking table
            EventLinkingTable = stream.Read<ushort>(EventCount);

            // Read the events
            Events = stream.Read<PC_R1_Event>(EventCount);

            // Read the event commands
            EventCommands = stream.Read<PC_R1_EventCommand>(EventCount);
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        public void Serialize(Stream stream)
        {
            // HEADER BLOCK

            // Write block pointer
            stream.Write(EventBlockPointer);
            stream.Write(TextureOffsetTablePointer);

            // Write map size
            stream.Write(MapWidth);
            stream.Write(MapHeight);

            // Write each palette
            foreach (var palette in ColorPalettes)
            {
                foreach (var color in palette.Reverse())
                {
                    // Write the palette color as RGB and divide by 4 (as the values are between 0-64)
                    stream.Write((byte)(color.Red / 4));
                    stream.Write((byte)(color.Green / 4));
                    stream.Write((byte)(color.Blue / 4));
                }
            }

            // Write unknown byte
            stream.Write(Unknown1);

            // MAP BLOCK

            // Write each map cell
            stream.Write(Tiles);

            // Write unknown byte
            stream.Write(Unknown2);

            // Write the background data
            stream.Write(BackgroundIndex);
            stream.Write(BackgroundSpritesDES);

            // Write the rough textures count
            stream.Write(RoughTextureCount);

            // Write the length of the third unknown value
            stream.Write(Unknown3Count);

            // Write each rough texture
            for (int i = 0; i < RoughTextureCount; i++)
                stream.Write(RoughTextures[i]);

            // Write the checksum for the rough textures
            stream.Write(RoughTexturesChecksum);

            // Write the index table for the rough textures
            stream.Write(RoughTexturesIndexTable);

            // Write the items for the third unknown value
            stream.Write(Unknown3);

            // Write the checksum for the third unknown value
            stream.Write(Unknown3Checksum);

            // Write the offset table for the third unknown value
            stream.Write(Unknown3OffsetTable);

            // TEXTURE BLOCK

            // Write the offset table for the textures
            stream.Write(TexturesOffsetTable);

            // Write the textures count
            stream.Write(TexturesCount);
            stream.Write(NonTransparentTexturesCount);
            stream.Write(TexturesDataTableCount);

            // Write the non-transparent textures
            foreach (var texture in NonTransparentTextures)
                // Write the texture
                stream.Write(texture);

            // Write the transparent textures
            foreach (var texture in TransparentTextures)
                // Write the texture
                stream.Write(texture);

            // Write the fourth unknown value
            stream.Write(Unknown4);

            // Write the checksum for the textures
            stream.Write(TexturesChecksum);

            // EVENT BLOCK

            // Write the event count
            stream.Write(EventCount);

            // Write the event linking table
            stream.Write(EventLinkingTable);

            // Write the events
            stream.Write(Events);

            // Write the event commands
            stream.Write(EventCommands);
        }

        #endregion
    }
}