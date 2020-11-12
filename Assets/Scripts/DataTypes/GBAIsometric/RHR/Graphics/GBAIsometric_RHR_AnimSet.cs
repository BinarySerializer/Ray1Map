using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class GBAIsometric_RHR_AnimSet : R1Serializable {
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte PivotX { get; set; }
        public byte PivotY { get; set; }
        public byte Byte_04 { get; set; }
        public byte AnimationCount { get; set; }
        public byte[] Bytes_06 { get; set; }
        public byte Flags { get; set; }
        public byte[] Bytes_09 { get; set; }
        public Pointer PalettePointer { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public Pointer FramesPointer { get; set; }
        public Pointer PatternsPointer { get; set; }
        public Pointer TileIndicesPointer { get; set; }
        public Pointer<GBAIsometric_RHR_GraphicsData> GraphicsDataPointer { get; set; }
        public byte[] Bytes_28 { get; set; }
        public Pointer NamePointer { get; set; }

        public ARGB1555Color[] Palette { get; set; }
        public string Name { get; set; }
        public GBAIsometric_RHR_Animation[] Animations { get; set; }
        public GBAIsometric_RHR_AnimFrame[] Frames { get; set; }
        public Dictionary<int, GBAIsometric_RHR_AnimPattern[]> Patterns { get; set; }
        public ushort[] TileIndices { get; set; }
        public bool Is8Bit => BitHelpers.ExtractBits(Flags, 1, 1) == 1;

        public override void SerializeImpl(SerializerObject s)
        {
            Height = s.Serialize<byte>(Height, name: nameof(Height));
            Width = s.Serialize<byte>(Width, name: nameof(Width));
            PivotX = s.Serialize<byte>(PivotX, name: nameof(PivotX));
            PivotY = s.Serialize<byte>(PivotY, name: nameof(PivotY));
            Byte_04 = s.Serialize<byte>(Byte_04, name: nameof(Byte_04));
            AnimationCount = s.Serialize<byte>(AnimationCount, name: nameof(AnimationCount));
            Bytes_06 = s.SerializeArray<byte>(Bytes_06, 2, name: nameof(Bytes_06));
            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
            Bytes_09 = s.SerializeArray<byte>(Bytes_09, 7, name: nameof(Bytes_09));
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            PatternsPointer = s.SerializePointer(PatternsPointer, name: nameof(PatternsPointer));
            TileIndicesPointer = s.SerializePointer(TileIndicesPointer, name: nameof(TileIndicesPointer));
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_RHR_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Bytes_28 = s.SerializeArray<byte>(Bytes_28, 8, name: nameof(Bytes_28));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            Palette = s.DoAt(PalettePointer, () => s.SerializeObjectArray<ARGB1555Color>(Palette, Is8Bit ? 256 : 16, name: nameof(Palette)));
            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));

            Animations = s.DoAt(AnimationsPointer, () => s.SerializeObjectArray<GBAIsometric_RHR_Animation>(Animations, AnimationCount, name: nameof(Animations)));
            Frames = s.DoAt(FramesPointer, () => s.SerializeObjectArray<GBAIsometric_RHR_AnimFrame>(Frames, Animations?.Max(a => a.StartFrameIndex + a.FrameCount) ?? 0, name: nameof(Frames)));
            if (Patterns == null) {
                Patterns = new Dictionary<int, GBAIsometric_RHR_AnimPattern[]>();
                foreach (var frame in Frames) {
                    if(frame.PatternIndex == 0xFFFF) continue;
                    if (!Patterns.ContainsKey(frame.PatternIndex)) {
                        var key = frame.PatternIndex;
                        List<GBAIsometric_RHR_AnimPattern> tempList = new List<GBAIsometric_RHR_AnimPattern>();
                        s.DoAt(PatternsPointer + 2 * key, () => {
                            GBAIsometric_RHR_AnimPattern curUnk = null;
                            while (curUnk == null || !curUnk.IsLastPattern) {
                                curUnk = s.SerializeObject<GBAIsometric_RHR_AnimPattern>(default, name: $"Patterns[{key}][{tempList.Count}]");
                                tempList.Add(curUnk);
                            }
                        });
                        Patterns[key] = tempList.ToArray();
                    }
                }
            }
            s.DoAt(TileIndicesPointer, () => {
                int numTiles = Frames.Where(f => f.PatternIndex != 0xFFFF).Max(f => f.TileIndicesIndex + Patterns[f.PatternIndex].Sum(p => p.NumTiles));
                TileIndices = s.SerializeArray<ushort>(TileIndices, numTiles, name: nameof(TileIndices));
            });
            /*s.Log($"{Name} - MaxIndex0: {Frames.Max(f => f.PatternIndex)} - MaxIndex1: {Frames.Max(f => f.TileIndicesIndex)}");
            s.Log($"{Name} - Max Unk: {Patterns.Max(usa => usa.Value.Max(us => us.Unknown))}");
            if (Name == "portraitRayman") {
                s.DoEncoded(new RHR_SpriteEncoder(true, GraphicsDataPointer.Value.CompressionLookupBuffer, GraphicsDataPointer.Value.CompressedDataPointer), () => {
                    byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));

                    var tex = Util.ToTileSetTexture(fullSheet, Palette.Select((x, i) => {
                        if (i != 0)
                            x.Alpha = 255;
                        return x.GetColor();
                    }).ToArray(), true, 8, true);

                    Util.ByteArrayToFile(Context.BasePath + $"animGroups/Full_0x{Offset.StringAbsoluteOffset}_{Name}.png", tex.EncodeToPNG());

                    for (int i = 0; i < TileIndices.Length; i++) {
                        byte[] arr = new byte[8*8];
                        Array.Copy(fullSheet, 8*8*TileIndices[i], arr, 0, arr.Length);

                        tex = Util.ToTileSetTexture(arr, Palette.Select((x, pi) => {
                            if (pi != 0)
                                x.Alpha = 255;
                            return x.GetColor();
                        }).ToArray(), true, 8, true);

                        Util.ByteArrayToFile(Context.BasePath + $"animGroups/{Name}/{i}.png", tex.EncodeToPNG());
                    }
                });
            }*/
        }
    }
}