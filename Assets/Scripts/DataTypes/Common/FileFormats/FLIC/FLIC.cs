using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using ImageMagick;
using UnityEngine;

namespace R1Engine
{
    // https://www.drdobbs.com/windows/the-flic-file-format/184408954
    // https://github.com/aseprite/flic/blob/main/decoder.cpp
    public class FLIC : BinarySerializable
    {
        public uint FileSize { get; set; }
        public FLIC_Format FormatType { get; set; }
        public ushort FramesCount { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ushort BitsPerPixel { get; set; }
        public ushort Flags { get; set; }
        public uint Speed { get; set; }
        public ushort Ushort_14 { get; set; }
        public uint CreationDate { get; set; }
        public uint CreationProgramSerialNum { get; set; }
        public uint UpdatedDate { get; set; }
        public uint UpdatedProgramSerialNum { get; set; }
        public ushort AspectX { get; set; }
        public ushort AspectY { get; set; }
        public byte[] Reserved_0 { get; set; }
        public Pointer FirstFramePointer { get; set; }
        public Pointer SecondFramePointer { get; set; }
        public byte[] Reserved_1 { get; set; }

        public FLIC_PrimaryChunk[] Chunks { get; set; }

        public MagickImageCollection ToMagickImageCollection()
        {
            // Convert frames to Magick images
            var magickFrames = new List<MagickImage>();

            // Create a texture
            Texture2D tex = TextureHelpers.CreateTexture2D(Width, Height);

            // Create a palette
            var palette = Enumerable.Repeat(Color.clear, 256).ToArray();

            // Enumerate every frame
            foreach (var frame in Chunks.Where(x => x.Chunk_Frame != null).Select(x => x.Chunk_Frame))
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
                        for (int y = 0; y < Height; y++)
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
                collection[index].AnimationDelay = (int)Speed;
                collection[index].AnimationTicksPerSecond = 1000;
                collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                index++;
            }

            return collection;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            FileSize = s.Serialize<uint>(FileSize, name: nameof(FileSize));
            FormatType = s.Serialize<FLIC_Format>(FormatType, name: nameof(FormatType));
            FramesCount = s.Serialize<ushort>(FramesCount, name: nameof(FramesCount));
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));
            BitsPerPixel = s.Serialize<ushort>(BitsPerPixel, name: nameof(BitsPerPixel));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            Speed = s.Serialize<uint>(Speed, name: nameof(Speed));
            Ushort_14 = s.Serialize<ushort>(Ushort_14, name: nameof(Ushort_14));
            CreationDate = s.Serialize<uint>(CreationDate, name: nameof(CreationDate));
            CreationProgramSerialNum = s.Serialize<uint>(CreationProgramSerialNum, name: nameof(CreationProgramSerialNum));
            UpdatedDate = s.Serialize<uint>(UpdatedDate, name: nameof(UpdatedDate));
            UpdatedProgramSerialNum = s.Serialize<uint>(UpdatedProgramSerialNum, name: nameof(UpdatedProgramSerialNum));
            AspectX = s.Serialize<ushort>(AspectX, name: nameof(AspectX));
            AspectY = s.Serialize<ushort>(AspectY, name: nameof(AspectY));
            Reserved_0 = s.SerializeArray<byte>(Reserved_0, 38, name: nameof(Reserved_0));
            FirstFramePointer = s.SerializePointer(FirstFramePointer, name: nameof(FirstFramePointer));
            SecondFramePointer = s.SerializePointer(SecondFramePointer, name: nameof(SecondFramePointer));
            Reserved_1 = s.SerializeArray<byte>(Reserved_1, 40, name: nameof(Reserved_1));

            Chunks = s.SerializeObjectArrayUntil(Chunks, x => s.CurrentPointer.FileOffset >= Offset.FileOffset + FileSize, includeLastObj: true, onPreSerialize: x => x.Flic = this, name: nameof(Chunks));
        }

        public enum FLIC_Format : ushort
        {
            FLI = 0xAF11,
            FLC = 0xAF12,
        }
    }
}