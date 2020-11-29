using System;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Vignette block group data for Rayman 1 (PS1)
    /// </summary>
    public class R1_PS1_VignetteBlockGroup : R1Serializable
    {
        /// <summary>
        /// The size of the block group, in pixels
        /// </summary>
        public int BlockGroupSize { get; set; }

        public ushort Unknown1 { get; set; }

        /// <summary>
        /// The image width
        /// </summary>
        public ushort Width { get; set; }

        /// <summary>
        /// The image height
        /// </summary>
        public ushort Height { get; set; }

        public ushort Unknown2 { get; set; }

        /// <summary>
        /// The image blocks, each one 64 pixels wide
        /// </summary>
        public RGBA5551Color[][] ImageBlocks { get; set; }

        /// <summary>
        /// Gets the block width based on engine version
        /// </summary>
        /// <param name="engineVersion">The engine version</param>
        /// <returns>The block width</returns>
        public int GetBlockWidth(EngineVersion engineVersion) => engineVersion == EngineVersion.R1_PS1_JPDemoVol3 ? 32 : 64;

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize header values
            Unknown1 = s.Serialize<ushort>(Unknown1, name: nameof(Unknown1));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            Unknown2 = s.Serialize<ushort>(Unknown2, name: nameof(Unknown2));

            // Get the block width
            var blockWidth = GetBlockWidth(s.GameSettings.EngineVersion);

            // Create block array
            if (ImageBlocks == null)
            {
                // Get the size of each block
                var blockSize = Height * blockWidth;

                ImageBlocks = new RGBA5551Color[BlockGroupSize / blockSize][];
            }

            // Serialize blocks
            for (int i = 0; i < ImageBlocks.Length; i++)
                ImageBlocks[i] = s.SerializeObjectArray<RGBA5551Color>(ImageBlocks[i], blockWidth * Height, name: nameof(ImageBlocks) + "[" + i + "]");
        }

        public Texture2D ToTexture(Context context)
        {
            // Create the texture
            var tex = TextureHelpers.CreateTexture2D(Width, Height);

            // Get the block width
            var blockWidth = GetBlockWidth(context.Settings.EngineVersion);

            // Write each block
            for (int blockIndex = 0; blockIndex < ImageBlocks.Length; blockIndex++)
            {
                // Get the block data
                var blockData = ImageBlocks[blockIndex];

                // Write the block
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < blockWidth; x++)
                    {
                        // Get the color
                        var c = blockData[x + (y * blockWidth)];

                        c.Alpha = Byte.MaxValue;

                        // Set the pixel
                        tex.SetPixel((x + (blockIndex * blockWidth)), tex.height - y - 1, c.GetColor());
                    }
                }
            }

            tex.Apply();

            return tex;
        }
    }
}