using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Image;
using ImageMagick;
using UnityEngine;

namespace Ray1Map
{
    public static class ImageExtensions
    {
        public static Texture2D ToTexture2D(this DDS_TextureItem img, bool invertY = false)
        {
            TextureFormat fmt = TextureFormat.RGBA32;
            if (img.Pre_Header?.PixelFormat?.FourCC == "ATI2")
            {
                fmt = TextureFormat.RGB24;
            }
            Texture2D bitmap = new Texture2D((int)img.Pre_Width, (int)img.Pre_Height, fmt, false);
            bitmap.LoadRawTextureData(img.ImageData);
            bitmap.Apply();
            if (invertY) {
                // invert y
                var pixels = bitmap.GetPixels();
                Color[] newPixels = new Color[pixels.Length];
                var w = bitmap.width;
                var h = bitmap.height;
                for (int y = 0; y < h; y++) {
                    for (int x = 0; x < w; x++) {
                        newPixels[y * w + x] = pixels[(h - 1 - y) * w + x];
                    }
                }
                pixels = newPixels;
                bitmap.SetPixels(pixels);
                bitmap.Apply();
            }
            return bitmap;
        }

        public static MagickImageCollection ToMagickImageCollection(this FLIC flic)
        {
            // Convert frames to Magick images
            var magickFrames = new List<MagickImage>();

            // Create a texture
            Texture2D tex = TextureHelpers.CreateTexture2D(flic.Width, flic.Height);

            // Create a palette
            var palette = Enumerable.Repeat(Color.clear, 256).ToArray();

            // Enumerate every frame
            foreach (var frame in flic.Chunks.Where(x => x.Chunk_Frame != null).Select(x => x.Chunk_Frame))
            {
                // Enumerate every frame chunk
                foreach (var chunk in frame.SubChunks)
                {
                    if (chunk.Color256 != null)
                    {
                        var palIndex = 0;

                        foreach (var packet in chunk.Color256.Packets)
                        {
                            palIndex += packet.Skip;

                            for (int j = 0; j < packet.Colors.Length; j++)
                                palette[palIndex + j] = packet.Colors[j].GetColor();

                            palIndex += packet.Colors.Length;
                        }
                    }
                    else if (chunk.ByteRun != null)
                    {
                        // Set every pixel in the texture
                        for (int y = 0; y < flic.Height; y++)
                        {
                            var line = chunk.ByteRun.Lines[y];

                            var x = 0;

                            foreach (var p in line.Packets.SelectMany(p => p.ImageData))
                            {
                                tex.SetPixel(x, tex.height - y - 1, palette[p]);
                                x++;
                            }
                        }
                    }
                    else if (chunk.DeltaFLC != null)
                    {
                        var y = 0;

                        // Modify pixels in the texture
                        for (int lineIndex = 0; lineIndex < chunk.DeltaFLC.LinesCount; lineIndex++)
                        {
                            var line = chunk.DeltaFLC.Lines[lineIndex];

                            foreach (var cmd in line.Commands)
                            {
                                // Skip lines
                                y += Math.Abs(cmd.Skip);

                                var x = 0;

                                foreach (var packet in cmd.Packets)
                                {
                                    x += packet.Skip;

                                    foreach (var p in packet.ImageData)
                                    {
                                        tex.SetPixel(x, tex.height - y - 1, palette[p]);
                                        x++;
                                    }
                                }

                                if (cmd.ValueType == 2)
                                    tex.SetPixel(x, tex.height - y - 1, palette[cmd.LastValue]);
                            }

                            y++;
                        }
                    }
                    else if (chunk.LiteralFLC != null)
                    {
                        var imgData = chunk.LiteralFLC.ImgData;

                        var p = 0;

                        for (int y = 0; y < flic.Height; y++)
                        {
                            for (int x = 0; x < flic.Width; x++)
                            {
                                tex.SetPixel(x, tex.height - y - 1, palette[imgData[p++]]);
                            }
                        }
                    }
                }

                tex.Apply();

                magickFrames.Add(tex.ToMagickImage());
            }

            // Save the frames as .gif files
            var collection = new MagickImageCollection();

            int index = 0;

            foreach (var img in magickFrames)
            {
                collection.Add(img);
                collection[index].AnimationDelay = (int)flic.Speed;
                collection[index].AnimationTicksPerSecond = 1000;
                collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                index++;
            }

            return collection;
        }

        public static Texture2D ToTexture(this PCX pcx, bool flip = false)
        {
            // Create the texture
            var tex = TextureHelpers.CreateTexture2D(pcx.ImageWidth, pcx.ImageHeight);

            // Set every pixel
            for (int y = 0; y < pcx.ImageHeight; y++)
            {
                for (int x = 0; x < pcx.ImageWidth; x++)
                {
                    // Get the palette index
                    var paletteIndex = pcx.ScanLines[y][x];

                    // Set the pixel
                    tex.SetPixel(x, flip ? (pcx.ImageHeight - y - 1) : y, pcx.VGAPalette[paletteIndex].GetColor());
                }
            }

            // Apply the pixels
            tex.Apply();

            // Return the texture
            return tex;
        }

        public static Texture2D ToTexture2D(this TGA tga)
        {
            if (tga.Header.OriginX != 0 || tga.Header.OriginY != 0)
                throw new NotImplementedException("Not implemented support for textures where origin is not 0");

            var tex = TextureHelpers.CreateTexture2D(tga.Header.Width, tga.Header.Height);

            switch (tga.Header.ImageType)
            {
                case TGA_ImageType.UnmappedRGB:
                case TGA_ImageType.UnmappedRGB_RLE:
                    tex.SetPixels(tga.RGBImageData.Select(x => x.GetColor()).ToArray());
                    break;

                default:
                    throw new NotImplementedException($"Not implemented support for textures with type {tga.Header.ImageType}");
            }

            tex.Apply();

            return tex;
        }
    }
}