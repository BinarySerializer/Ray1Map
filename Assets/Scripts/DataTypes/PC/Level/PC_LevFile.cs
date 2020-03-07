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
        public byte Unknown1 { get; set; }

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

        #endregion

        #region Public Methods

        /// <summary>
        /// Deserializes the file contents
        /// </summary>
        /// <param name="deserializer">The deserializer</param>
        public override void Deserialize(BinaryDeserializer deserializer)
        {
            // PC HEADER

            base.Deserialize(deserializer);

            // HEADER BLOCK

            // Read block pointer
            EventBlockPointer = deserializer.Read<uint>();
            TextureBlockPointer = deserializer.Read<uint>();

            // TODO: Does this contain the level name + description?
            if (deserializer.GameSettings.GameMode != GameMode.RayPC)
                Unknown6 = deserializer.ReadArray<byte>(68);

            // Read map size
            Width = deserializer.Read<ushort>();
            Height = deserializer.Read<ushort>();

            // Create the palettes
            ColorPalettes = deserializer.GameSettings.GameMode == GameMode.RayKit ? new ARGBColor[][]
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
                    palette[i] = new ARGBColor((byte)(deserializer.Read<byte>() * 4), (byte)(deserializer.Read<byte>() * 4),
                        (byte)(deserializer.Read<byte>() * 4));
                }

                // Reverse the palette
                ColorPalettes[paletteIndex] = palette;
            }

            // Read unknown byte
            Unknown1 = deserializer.Read<byte>();

            // MAP BLOCK

            // Create the collection of map cells
            Tiles = new PC_MapTile[Width * Height];

            // Read each map cell
            Tiles = deserializer.ReadArray<PC_MapTile>((ulong)Height * Width);
            // TODO: Update all below changes for other versions when writing
            if (deserializer.GameSettings.GameMode == GameMode.RayPC)
            {
                // Read unknown byte
                Unknown2 = deserializer.Read<byte>();

                // Read the background data
                BackgroundIndex = deserializer.Read<byte>();
                BackgroundSpritesDES = deserializer.Read<uint>();

                // Read the rough textures count
                RoughTextureCount = deserializer.Read<uint>();

                // Read the length of the third unknown value
                Unknown3Count = deserializer.Read<uint>();

                // Create the collection of rough textures
                RoughTextures = new byte[RoughTextureCount][];

                // Read each rough texture
                for (int i = 0; i < RoughTextureCount; i++)
                    RoughTextures[i] = deserializer.ReadArray<byte>(PC_Manager.CellSize * PC_Manager.CellSize);

                // Read the checksum for the rough textures
                RoughTexturesChecksum = deserializer.Read<byte>();

                // Read the index table for the rough textures
                RoughTexturesIndexTable = deserializer.ReadArray<uint>(1200);

                // Read the items for the third unknown value
                Unknown3 = deserializer.ReadArray<byte>(Unknown3Count);

                // Read the checksum for the third unknown value
                Unknown3Checksum = deserializer.Read<byte>();

                // Read the offset table for the third unknown value
                Unknown3OffsetTable = deserializer.ReadArray<uint>(1200);
            }
            else
            {
                // Read unknown values
                Unknown7 = deserializer.ReadArray<byte>((ulong)(TextureBlockPointer - deserializer.BaseStream.Position));
            }

            // TEXTURE BLOCK

            // At this point the stream position should match the texture block offset
            if (deserializer.BaseStream.Position != TextureBlockPointer)
                Debug.LogError("Texture block offset is incorrect");

            if (deserializer.GameSettings.GameMode != GameMode.RayPC)
                TextureBlockChecksum = deserializer.Read<byte>();

            // Get the xor key to use for the texture block
            byte texXor = (byte)(deserializer.GameSettings.GameMode == GameMode.RayPC ? 0 : 255);

            // Read the offset table for the textures
            TexturesOffsetTable = deserializer.ReadArray<uint>(1200, texXor);

            // Read the textures count
            TexturesCount = deserializer.Read<uint>(texXor);
            NonTransparentTexturesCount = deserializer.Read<uint>(texXor);
            TexturesDataTableCount = deserializer.Read<uint>(texXor);

            // Get the current offset to use for the texture offsets
            var textureBaseOffset = deserializer.BaseStream.Position;

            // Create the collection of non-transparent textures
            NonTransparentTextures = new PC_TileTexture[NonTransparentTexturesCount];

            // Read the non-transparent textures
            for (int i = 0; i < NonTransparentTextures.Length; i++)
            {
                // Create the texture
                var t = new PC_TileTexture()
                {
                    // Set the offset
                    Offset = (uint)(deserializer.BaseStream.Position - textureBaseOffset)
                };

                // Deserialize the texture
                t.Deserialize(deserializer);

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
                    Offset = (uint)(deserializer.BaseStream.Position - textureBaseOffset)
                };

                // Deserialize the texture
                t.Deserialize(deserializer);

                // Add the texture to the collection
                TransparentTextures[i] = t;
            }

            // Read the fourth unknown value
            Unknown4 = deserializer.ReadArray<byte>(32);

            if (deserializer.GameSettings.GameMode == GameMode.RayPC)
            {
                // Read the checksum for the textures
                TexturesChecksum = deserializer.Read<byte>();
            }

            // EVENT BLOCK

            // At this point the stream position should match the event block offset
            if (deserializer.BaseStream.Position != EventBlockPointer)
                Debug.LogError("Event block offset is incorrect");

            if (deserializer.GameSettings.GameMode != GameMode.RayPC)
                EventBlockChecksum = deserializer.Read<byte>();

            // Get the xor key to use for the event block
            byte eveXor = (byte)(deserializer.GameSettings.GameMode == GameMode.RayPC ? 0 : 145);

            // Read the event count
            EventCount = deserializer.Read<ushort>(eveXor);

            // Read the event linking table
            EventLinkingTable = deserializer.ReadArray<ushort>(EventCount, eveXor);

            // Read the events
            Events = deserializer.ReadArray<PC_Event>(EventCount);

            // Read the event commands
            EventCommands = deserializer.ReadArray<PC_EventCommand>(EventCount);

            Debug.Log($"PC R1 level loaded with size {Width}x{Height} and {EventCount} events");
        }

        /// <summary>
        /// Serializes the file contents
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void Serialize(BinarySerializer serializer)
        {
            // PC HEADER

            base.Serialize(serializer);

            // HEADER BLOCK

            // Write block pointer
            serializer.Write(EventBlockPointer);
            serializer.Write(TextureBlockPointer);

            if (serializer.GameSettings.GameMode != GameMode.RayPC)
                serializer.Write(Unknown6);

            // Write map size
            serializer.Write(Width);
            serializer.Write(Height);

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

            // Write unknown byte
            serializer.Write(Unknown1);

            // MAP BLOCK

            // Write each map cell
            serializer.Write(Tiles);

            // Write unknown byte
            serializer.Write(Unknown2);

            // Write the background data
            serializer.Write(BackgroundIndex);
            serializer.Write(BackgroundSpritesDES);

            // Write the rough textures count
            serializer.Write(RoughTextureCount);

            // Write the length of the third unknown value
            serializer.Write(Unknown3Count);

            // Write each rough texture
            for (int i = 0; i < RoughTextureCount; i++)
                serializer.Write(RoughTextures[i]);

            // Write the checksum for the rough textures
            serializer.Write(RoughTexturesChecksum);

            // Write the index table for the rough textures
            serializer.Write(RoughTexturesIndexTable);

            // Write the items for the third unknown value
            serializer.Write(Unknown3);

            // Write the checksum for the third unknown value
            serializer.Write(Unknown3Checksum);

            // Write the offset table for the third unknown value
            serializer.Write(Unknown3OffsetTable);

            // TEXTURE BLOCK

            // Write the offset table for the textures
            serializer.Write(TexturesOffsetTable);

            // Write the textures count
            serializer.Write(TexturesCount);
            serializer.Write(NonTransparentTexturesCount);
            serializer.Write(TexturesDataTableCount);

            // Write the non-transparent textures
            foreach (var texture in NonTransparentTextures)
                // Write the texture
                serializer.Write(texture);

            // Write the transparent textures
            foreach (var texture in TransparentTextures)
                // Write the texture
                serializer.Write(texture);

            // Write the fourth unknown value
            serializer.Write(Unknown4);

            // Write the checksum for the textures
            serializer.Write(TexturesChecksum);

            // EVENT BLOCK

            // Write the event count
            serializer.Write(EventCount);

            // Write the event linking table
            serializer.Write(EventLinkingTable);

            // Write the events
            serializer.Write(Events);

            // Write the event commands
            serializer.Write(EventCommands);
        }

        #endregion
    }
}