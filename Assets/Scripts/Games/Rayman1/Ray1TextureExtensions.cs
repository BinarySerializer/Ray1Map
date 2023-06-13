using System;
using BinarySerializer;
using BinarySerializer.Ray1;
using BinarySerializer.Ray1.PS1;
using UnityEngine;

namespace Ray1Map.Rayman1
{
    public static class Ray1TextureExtensions
    {
        public static Texture2D ToTexture(this Fond vig, Context context)
        {
            // Create the texture
            var tex = TextureHelpers.CreateTexture2D(vig.Width, vig.Height);

            // Get the block width
            var blockWidth = vig.GetBlockWidth(context.GetRequiredSettings<Ray1Settings>().EngineVersion);

            // Write each block
            for (int blockIndex = 0; blockIndex < vig.ImageBlocks.Length; blockIndex++)
            {
                // Get the block data
                var blockData = vig.ImageBlocks[blockIndex];

                // Write the block
                for (int y = 0; y < vig.Height; y++)
                {
                    for (int x = 0; x < blockWidth; x++)
                    {
                        // Get the color
                        var c = blockData.ImageData[x + (y * blockWidth)];

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