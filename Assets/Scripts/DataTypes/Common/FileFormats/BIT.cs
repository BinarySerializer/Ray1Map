using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// Sega Saturn .BIT file data
    /// </summary>
    public class BIT : R1Serializable
    {
        public RGBA5551Color[] Pixels;
        public byte[] PixelsPaletted;

        public Array<byte> PLT;
        public ObjectArray<RGBA5551Color> PAL;

        /// <summary>
        /// Converts the PCX data to a texture
        /// </summary>
        /// <returns>The texture</returns>
        public Texture2D ToTexture(int width, int blockWidth = 8, int blockHeight = 8, bool swizzled = true, bool invertYAxis = false)
        {
            int height = (PixelsPaletted?.Length ?? Pixels.Length) / width;

            // Create the texture
            var tex = TextureHelpers.CreateTexture2D(width, height, clear: true);

            var blockSwizzlePattern = new Coordinate[] 
            {
                new Coordinate(0,0),
                new Coordinate(1,0),
                new Coordinate(0,1),
                new Coordinate(1,1)
            };

            // Set every pixel
            int curPixel = 0;
            int xStep, yStep;
            if (swizzled) {
                xStep = blockSwizzlePattern.Max(c => c.x) + 1;
                yStep = blockSwizzlePattern.Max(c => c.y) + 1;
                if (width < xStep * blockWidth) {
                    blockSwizzlePattern = blockSwizzlePattern.Where(c => c.x * blockWidth < width).ToArray();
                }
                if (height < yStep * blockWidth) {
                    blockSwizzlePattern = blockSwizzlePattern.Where(c => c.y * blockHeight < height).ToArray();
                }
                xStep = blockSwizzlePattern.Max(c => c.x) + 1;
                yStep = blockSwizzlePattern.Max(c => c.y) + 1;
            } else {
                blockSwizzlePattern = new Coordinate[] { new Coordinate(0, 0) };
                xStep = 1;
                yStep = 1;
            }
            for (int y = 0; y < height / blockHeight / yStep; y++) {
                for (int x = 0; x < width / blockWidth / xStep; x++) {
                    for (int b = 0; b < blockSwizzlePattern.Length; b++) {
                        int Bx = ((x * xStep) + blockSwizzlePattern[b].x) * blockWidth;
                        int By = ((y * yStep) + blockSwizzlePattern[b].y) * blockHeight;
                        if (Bx >= width) continue;
                        if (By >= height) continue;
                        for (int by = 0; by < blockHeight; by++) {
                            for (int bx = 0; bx < blockWidth; bx++) {
                                if (Bx + bx >= width) continue;
                                if (By + by >= height) continue;
                                if (PixelsPaletted != null) {
                                    byte index = PixelsPaletted[curPixel];
                                    int pltIndex = ((y * yStep)) * blockHeight + x;
                                    if (pltIndex < PLT.Value.Length) {
                                        byte numPalette = PLT.Value[pltIndex];
                                        uint palIndex = 256 * (uint)numPalette + index;
                                        Color col = PAL.Value[palIndex].GetColor(); // 555? huh?
                                        tex.SetPixel(Bx + bx, invertYAxis ? height - 1 - (By + by) : (By + by), new Color(col.r, col.g, col.b, 1f));
                                        //tex.SetPixel(Bx + bx, height - 1 - (By + by), new Color(index / 255f, index / 255f, index / 255f, 1f));
                                    }
                                } else {
                                    tex.SetPixel(Bx + bx, invertYAxis ? height - 1 - (By + by) : (By + by), Pixels[curPixel].GetColor());
                                }
                                curPixel++;
                            }
                        }
                    }
                }
            }

            // Apply the pixels
            tex.Apply();

            // Return the texture
            return tex;
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s) {
            if (PAL != null) {
                if (PixelsPaletted == null) {
                    PixelsPaletted = new byte[s.CurrentLength];
                }
                PixelsPaletted = s.SerializeArray<byte>(PixelsPaletted, PixelsPaletted.Length, name: nameof(PixelsPaletted));
            } else {
                if (Pixels == null) {
                    Pixels = new RGBA5551Color[s.CurrentLength / 2];
                }
                Pixels = s.SerializeObjectArray<RGBA5551Color>(Pixels, Pixels.Length, name: nameof(Pixels));
            }
        }

        private struct Coordinate {
            public int x;
            public int y;

            public Coordinate(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }
    }
}