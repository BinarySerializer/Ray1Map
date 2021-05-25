using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;


namespace R1Engine
{
    public class GBAVV_Mode7_LevelInfo : BinarySerializable
    {
        public bool SerializeData { get; set; } // Set before serializing

        public uint LevelType { get; set; }
        public Pointer TileSetFramesPointer { get; set; }
        public uint TileSetFramesBlockLength { get; set; }
        public Pointer Crash1_BackgroundPointer { get; set; }
        public uint Crash2_BackgroundIndex { get; set; }
        public Pointer ObjPalettePointer { get; set; }
        public Pointer ObjDataPointer { get; set; }
        public Pointer AnimSetsPointer { get; set; }
        public Pointer ObjGraphicsPointer { get; set; }
        public uint Uint_20 { get; set; }
        public uint Uint_24 { get; set; }
        public uint Uint_28 { get; set; }
        public uint Uint_2C { get; set; }
        public uint Uint_30 { get; set; }

        // Serialized from pointers
        public GBAVV_Mode7_TileFrames TileSetFrames { get; set; }
        public GBAVV_Mode7_Background Crash1_Background { get; set; }
        public RGBA5551Color[] ObjPalette { get; set; }
        public GBAVV_Mode7_ObjData ObjData { get; set; }
        public GBAVV_Mode7_AnimSet[] AnimSets { get; set; }
        public GBAVV_Mode7_AnimSet[] SpongeBob_SpecialAnimSets { get; set; } // HUD
        public GBAVV_Mode7_ObjGraphicsBlock ObjGraphics { get; set; }

        public GBAVV_Mode7_AnimSet AnimSet_Chase { get; set; } // Bear in Crash 1, Shark in Crash 2

        public RGBA5551Color[] Crash1_PolarDeathPalette { get; set; }

        public IEnumerable<GBAVV_Mode7_AnimSet> GetAllAnimSets => AnimSets.Concat(SpongeBob_SpecialAnimSets ?? new GBAVV_Mode7_AnimSet[0]).Append(AnimSet_Chase).Where(x => x != null);

        // The special frames for the blimp and N. Gin in Crash 1. Animations are at 0x0817a534 and 0x0817a60c. These are stored as 4-bit graphics, but get converted to 8-bit in memory.
        public GBAVV_Mode7_SpecialFrames SpecialFrames { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            if (s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
                LevelType = s.Serialize<uint>(LevelType, name: nameof(LevelType));

            TileSetFramesPointer = s.SerializePointer(TileSetFramesPointer, name: nameof(TileSetFramesPointer));
            TileSetFramesBlockLength = s.Serialize<uint>(TileSetFramesBlockLength, name: nameof(TileSetFramesBlockLength));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1 || s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
                Crash1_BackgroundPointer = s.SerializePointer(Crash1_BackgroundPointer, name: nameof(Crash1_BackgroundPointer));
            else
                Crash2_BackgroundIndex = s.Serialize<uint>(Crash2_BackgroundIndex, name: nameof(Crash2_BackgroundIndex));

            ObjPalettePointer = s.SerializePointer(ObjPalettePointer, name: nameof(ObjPalettePointer));
            ObjDataPointer = s.SerializePointer(ObjDataPointer, name: nameof(ObjDataPointer));
            AnimSetsPointer = s.SerializePointer(AnimSetsPointer, name: nameof(AnimSetsPointer));
            ObjGraphicsPointer = s.SerializePointer(ObjGraphicsPointer, name: nameof(ObjGraphicsPointer));

            if (s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
            {
                Uint_20 = s.Serialize<uint>(Uint_20, name: nameof(Uint_20));
                Uint_24 = s.Serialize<uint>(Uint_24, name: nameof(Uint_24));
                Uint_28 = s.Serialize<uint>(Uint_28, name: nameof(Uint_28));
                Uint_2C = s.Serialize<uint>(Uint_2C, name: nameof(Uint_2C));
                Uint_30 = s.Serialize<uint>(Uint_30, name: nameof(Uint_30));
            }

            int animSetsCount = s.GetR1Settings().GetGameManagerOfType<GBAVV_Generic_BaseManager>().Mode7AnimSetCounts[LevelType];

            // Serialize animation sets
            AnimSets = s.DoAt(AnimSetsPointer, () => s.SerializeObjectArray<GBAVV_Mode7_AnimSet>(AnimSets, animSetsCount, name: nameof(AnimSets)));

            s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Mode7_SpongeBob_SpecialAnimSets), () =>
            {
                var framesCounts = new int[] { 1, 12, 12, 1, 9, 9, 9, 9, 9, 5 };

                if (SpongeBob_SpecialAnimSets == null)
                    SpongeBob_SpecialAnimSets = new GBAVV_Mode7_AnimSet[4 + 6];

                for (int i = 0; i < SpongeBob_SpecialAnimSets.Length; i++)
                    SpongeBob_SpecialAnimSets[i] = s.SerializeObject(SpongeBob_SpecialAnimSets[i], x =>
                    {
                        x.IsSpongeBobSpecialAnim = true;
                        x.OverrideFramesCount = framesCounts[i];
                    }, name: $"{nameof(SpongeBob_SpecialAnimSets)}[{i}]");
            });

            if (!SerializeData)
                return;

            TileSetFrames = s.DoAt(TileSetFramesPointer, () => s.SerializeObject<GBAVV_Mode7_TileFrames>(TileSetFrames, x =>
            {
                x.TileSetFramesBlockLength = TileSetFramesBlockLength;
                x.HasPaletteIndices = LevelType == 0 && s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1;
            }, name: nameof(TileSetFrames)));

            s.DoAt(Crash1_BackgroundPointer, () =>
            {
                s.DoEncodedIf(new GBA_LZSSEncoder(), s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman, () => Crash1_Background = s.SerializeObject<GBAVV_Mode7_Background>(Crash1_Background, name: nameof(Crash1_Background)));
            });

            ObjPalette = s.DoAt(ObjPalettePointer, () => s.SerializeObjectArray<RGBA5551Color>(ObjPalette, 256, name: nameof(ObjPalette)));
            ObjData = s.DoAt(ObjDataPointer, () => s.SerializeObject<GBAVV_Mode7_ObjData>(ObjData, name: nameof(ObjData)));

            if (LevelType == 0 && s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_SpongeBobRevengeOfTheFlyingDutchman)
            {
                AnimSet_Chase = new GBAVV_Mode7_AnimSet()
                {
                    AnimationsPointer = pointerTable[GBAVV_Pointer.Mode7_Type0_ChaseObjAnimations],
                    FrameOffsetsPointer = pointerTable[GBAVV_Pointer.Mode7_Type0_ChaseObjFrames],
                    PaletteIndex = (uint)(s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1 ? 0x1F : 0x12), // Tile pal 0x0F and 0x02
                };
            }

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1 && LevelType == 1)
                // Load the blimp
                SpecialFrames = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash1_Type1_SpecialFrame], () => s.SerializeObject<GBAVV_Mode7_SpecialFrames>(SpecialFrames, x => x.FramesCount = 4, name: nameof(SpecialFrames)));
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1 && LevelType == 2)
                // Load N. Gin
                SpecialFrames = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash1_Type2_SpecialFrame], () => s.SerializeObject<GBAVV_Mode7_SpecialFrames>(SpecialFrames, x => x.FramesCount = 1, name: nameof(SpecialFrames)));

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_Crash1)
                Crash1_PolarDeathPalette = s.DoAt(pointerTable[GBAVV_Pointer.Mode7_Crash1_PolarDeathPalette], () => s.SerializeObjectArray<RGBA5551Color>(Crash1_PolarDeathPalette, 16, name: nameof(Crash1_PolarDeathPalette)));
        }

        public void SerializeAnimations(SerializerObject s, IEnumerable<GBAVV_Mode7_AnimSet> animSets)
        {
            // Get all pointers used by the animation sets
            var pointers = animSets.SelectMany(x => new Pointer[]
            {
                x.FrameOffsetsPointer, x.AnimationsPointer
            }).Where(x => x != null).Distinct().OrderBy(x => x.AbsoluteOffset).ToArray();

            var a = GetAllAnimSets.ToArray();

            // Serialize animations in animation sets
            for (var i = 0; i < a.Length; i++)
                serializeAnimSet(a[i], i);

            void serializeAnimSet(GBAVV_Mode7_AnimSet animSet, int index)
            {
                // Ignore null animations
                if (animSet?.AnimationsPointer == null)
                    return;

                var length = (pointers.First(x => x.AbsoluteOffset > animSet.AnimationsPointer.AbsoluteOffset) - animSet.AnimationsPointer) / 12;

                animSet.SerializeAnimations(s, (int)length, index);
            }

            ObjGraphics = s.DoAt(ObjGraphicsPointer, () => s.SerializeObject<GBAVV_Mode7_ObjGraphicsBlock>(ObjGraphics, x => x.AnimSets = a, name: nameof(ObjGraphics)));
        }
    }
}