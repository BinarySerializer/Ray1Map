using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_RHR_AnimSet : R1Serializable {
        public byte[] Bytes_00 { get; set; }
        public byte AnimationCount { get; set; }
        public byte[] Bytes_06 { get; set; }
        public Pointer PalettePointer { get; set; }
        public Pointer AnimationsPointer { get; set; }
        public Pointer FramesPointer { get; set; }
        public Pointer UnkStructsPointer { get; set; } // Maybe compressed in a way.
        public Pointer TileIndicesPointer { get; set; } // TODO: get the length
        public Pointer<GBAIsometric_RHR_GraphicsData> GraphicsDataPointer { get; set; }
        public byte[] Bytes_28 { get; set; }
        public Pointer NamePointer { get; set; }

        public ARGB1555Color[] Palette { get; set; }
        public string Name { get; set; }
        public GBAIsometric_RHR_Animation[] Animations { get; set; }
        public GBAIsometric_RHR_AnimFrame[] Frames { get; set; }
        public Dictionary<int, GBAIsometric_RHR_AnimSet_Unk[]> UnkStructs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Bytes_00 = s.SerializeArray<byte>(Bytes_00, 5, name: nameof(Bytes_00));
            AnimationCount = s.Serialize<byte>(AnimationCount, name: nameof(AnimationCount));
            Bytes_06 = s.SerializeArray<byte>(Bytes_06, 10, name: nameof(Bytes_06));
            PalettePointer = s.SerializePointer(PalettePointer, name: nameof(PalettePointer));
            AnimationsPointer = s.SerializePointer(AnimationsPointer, name: nameof(AnimationsPointer));
            FramesPointer = s.SerializePointer(FramesPointer, name: nameof(FramesPointer));
            UnkStructsPointer = s.SerializePointer(UnkStructsPointer, name: nameof(UnkStructsPointer));
            TileIndicesPointer = s.SerializePointer(TileIndicesPointer, name: nameof(TileIndicesPointer));
            GraphicsDataPointer = s.SerializePointer<GBAIsometric_RHR_GraphicsData>(GraphicsDataPointer, resolve: true, name: nameof(GraphicsDataPointer));
            Bytes_28 = s.SerializeArray<byte>(Bytes_28, 8, name: nameof(Bytes_28));
            NamePointer = s.SerializePointer(NamePointer, name: nameof(NamePointer));

            Palette = s.DoAt(PalettePointer, () => s.SerializeObjectArray<ARGB1555Color>(Palette, 16, name: nameof(Palette)));
            Name = s.DoAt(NamePointer, () => s.SerializeString(Name, name: nameof(Name)));

            Animations = s.DoAt(AnimationsPointer, () => s.SerializeObjectArray<GBAIsometric_RHR_Animation>(Animations, AnimationCount, name: nameof(Animations)));
            Frames = s.DoAt(FramesPointer, () => s.SerializeObjectArray<GBAIsometric_RHR_AnimFrame>(Frames, Animations?.Max(a => a.StartFrameIndex + a.FrameCount) ?? 0, name: nameof(Frames)));
            if (UnkStructs == null) {
                UnkStructs = new Dictionary<int, GBAIsometric_RHR_AnimSet_Unk[]>();
                foreach (var frame in Frames) {
                    if (!UnkStructs.ContainsKey(frame.UnkStructIndex)) {
                        var key = frame.UnkStructIndex;
                        List<GBAIsometric_RHR_AnimSet_Unk> tempList = new List<GBAIsometric_RHR_AnimSet_Unk>();
                        s.DoAt(UnkStructsPointer + 2 * key, () => {
                            GBAIsometric_RHR_AnimSet_Unk curUnk = null;
                            while (curUnk == null || !curUnk.IsLastUnk) {
                                curUnk = s.SerializeObject<GBAIsometric_RHR_AnimSet_Unk>(default, name: $"UnkStructs[{key}][{tempList.Count}]");
                                tempList.Add(curUnk);
                            }
                        });
                        UnkStructs[key] = tempList.ToArray();
                    }
                }
            }
            s.Log($"MaxIndex0: {Frames.Max(f => f.UnkStructIndex)} - MaxIndex1: {Frames.Max(f => f.TileIndicesIndex)}");
            //s.DoEncoded(new RHR_SpriteEncoder(false, GraphicsDataPointer.Value.CompressionLookupBuffer, GraphicsDataPointer.Value.CompressedDataPointer), () => {
            //    byte[] fullSheet = s.SerializeArray<byte>(default, s.CurrentLength, name: nameof(fullSheet));

            //    var tex = Util.ToTileSetTexture(fullSheet, Palette.Select((x, i) =>
            //    {
            //        if (i != 0)
            //            x.Alpha = 255;
            //        return x.GetColor();
            //    }).ToArray(), false, 8, true);

            //    Util.ByteArrayToFile(Context.BasePath + $"animGroups/Full_0x{Offset.AbsoluteOffset:X8}_{Name}.png", tex.EncodeToPNG());
            //});
        }
    }
}