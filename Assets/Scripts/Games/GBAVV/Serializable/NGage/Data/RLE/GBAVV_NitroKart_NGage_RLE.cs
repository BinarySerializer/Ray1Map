using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GBAVV
{
    public class GBAVV_NitroKart_NGage_RLE : BinarySerializable
    {
        public string Magic { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FramesCount { get; set; }
        public Pointer[] FramePointers { get; set; }

        // Serialized from pointers
        public GBAVV_NitroKart_NGage_RLEFrame[] Frames { get; set; }

        // Helpers
        public Texture2D[] ToTextures(GBAVV_NitroKart_NGage_PAL palette, bool flipY = true)
        {
            var textures = new Texture2D[FramesCount];

            // Convert palette
            var pal = palette.Palette.Select((x, i) =>
            {
                if (i == 0)
                    return Color.clear;

                var c = x.GetColor();
                return new Color(c.r, c.g, c.b);
            }).ToArray();

            for (int i = 0; i < FramesCount; i++)
            {
                // Create the texture
                var tex = TextureHelpers.CreateTexture2D(Width, Height);

                // Helper
                void setPixel(int x, int y, byte? c = null) => tex.SetPixel(x, flipY ? Height - y - 1 : y, c == null ? Color.clear : pal[c.Value / 2]);

                // Get the frame
                var frame = Frames[i];

                // Enumerate every row
                for (int y = 0; y < Height; y++)
                {
                    var row = frame.Rows[y];

                    // Fill every pixel
                    if (row.CommandsOffset == 1)
                    {
                        // Empty
                        for (int x = 0; x < Width; x++)
                            setPixel(x, y);
                    }
                    else if (row.CommandsOffset == 2)
                    {
                        // Repeat
                        for (int x = 0; x < Width; x++)
                            setPixel(x, y, row.ImgageData[0]);
                    }
                    else
                    {
                        var x = 0;
                        var imgOffset = 0;

                        foreach (var cmd in row.Commands)
                        {
                            if (cmd.Type == 0)
                            {
                                // Fill array
                                for (int j = 0; j < cmd.Count; j++)
                                {
                                    setPixel(x, y, row.ImgageData[imgOffset]);
                                    imgOffset++;
                                    x++;
                                }
                            }
                            else if (cmd.Type == 1)
                            {
                                // Empty
                                for (int j = 0; j < cmd.Count; j++)
                                {
                                    setPixel(x, y);
                                    x++;
                                }
                            }
                            else if (cmd.Type == 2)
                            {
                                // Repeat
                                for (int j = 0; j < cmd.Count; j++)
                                {
                                    setPixel(x, y, row.ImgageData[imgOffset]);
                                    x++;
                                }

                                imgOffset++;
                            }
                        }
                    }
                }

                tex.Apply();

                textures[i] = tex;
            }

            return textures;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            Width = s.Serialize<int>(Width, name: nameof(Width));
            Height = s.Serialize<int>(Height, name: nameof(Height));
            FramesCount = s.Serialize<int>(FramesCount, name: nameof(FramesCount));
            FramePointers = s.SerializePointerArray(FramePointers, FramesCount, name: nameof(FramePointers));

            if (Frames == null)
                Frames = new GBAVV_NitroKart_NGage_RLEFrame[FramesCount];

            for (int i = 0; i < Frames.Length; i++)
                Frames[i] = s.DoAt(FramePointers[i], () => s.SerializeObject<GBAVV_NitroKart_NGage_RLEFrame>(Frames[i], x =>
                {
                    x.Width = Width;
                    x.Height = Height;
                }, name: $"{nameof(Frames)}[{i}]"));
            
            s.Goto(Offset + s.CurrentLength);
        }
    }
}