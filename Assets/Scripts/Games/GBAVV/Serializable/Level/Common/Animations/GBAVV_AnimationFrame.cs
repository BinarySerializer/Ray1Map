using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_AnimationFrame : BinarySerializable
    {
        public Pointer TilePositionsPointer { get; set; }
        public Pointer TileShapesPointer { get; set; }
        public UInt24 TileOffset { get; set; } // Offset in the global tileset
        public byte TilesCount { get; set; }

        // Nitro Kart
        public Pointer NitroKart_Pointer_10 { get; set; } // 2 shorts, same as Fusion_Pointer_28?

        // Nitro Kart (N-Gage)
        public Pointer NitroKart_NGage_ImageDataPointer { get; set; }
        public int NitroKart_NGage_Width => RenderBox.Width + 2; // TODO: Why is this needed? Is it always +2 for a rect?
        public int NitroKart_NGage_Height => RenderBox.Height + 2;

        // Fusion
        public Pointer Fusion_TileSetPointer { get; set; }
        public GBAVV_AnimationRect RenderBox { get; set; }
        public byte[] Fusion_Data_11 { get; set; }
        public Pointer Fusion_Pointer_14 { get; set; } // 8 bytes
        public Pointer Fusion_HitBox1Pointer { get; set; }
        public Pointer Fusion_HitBox2Pointer { get; set; }
        public Pointer Fusion_HitBox3Pointer { get; set; }
        public Pointer Fusion_Pointer_24 { get; set; } // Always null
        public Pointer Fusion_Pointer_28 { get; set; } // 2 shorts

        // Serialized from pointers
        public TilePosition[] TilePositions { get; set; }
        public TileShape[] TileShapes { get; set; }

        // Nitro Kart (N-Gage)
        public byte[] NitroKart_NGage_ImageData { get; set; }

        // Fusion
        public byte[] Fusion_TileSet { get; set; }
        public GBAVV_AnimationRect Fusion_HitBox1 { get; set; }
        public GBAVV_AnimationRect Fusion_HitBox2 { get; set; }
        public GBAVV_AnimationRect Fusion_HitBox3 { get; set; }

        // Helpers
        public int GetTileShape(int index)
        {
            if (Context.GetR1Settings().EngineVersion >= EngineVersion.GBAVV_CrashFusion && Context.GetR1Settings().EngineVersion != EngineVersion.GBAVV_KidsNextDoorOperationSODA)
                return TilePositions[index].ShapeIndex;
            else
                return TileShapes[index].ShapeIndex;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GetR1Settings().EngineVersion >= EngineVersion.GBAVV_CrashFusion && s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_KidsNextDoorOperationSODA)
            {
                TilePositionsPointer = s.SerializePointer(TilePositionsPointer, name: nameof(TilePositionsPointer));
                Fusion_TileSetPointer = s.SerializePointer(Fusion_TileSetPointer, name: nameof(Fusion_TileSetPointer));
                RenderBox = s.SerializeObject<GBAVV_AnimationRect>(RenderBox, name: nameof(RenderBox));
                TilesCount = s.Serialize<byte>(TilesCount, name: nameof(TilesCount));
                Fusion_Data_11 = s.SerializeArray<byte>(Fusion_Data_11, 3, name: nameof(Fusion_Data_11)); // Padding?
                Fusion_Pointer_14 = s.SerializePointer(Fusion_Pointer_14, name: nameof(Fusion_Pointer_14));
                Fusion_HitBox1Pointer = s.SerializePointer(Fusion_HitBox1Pointer, name: nameof(Fusion_HitBox1Pointer));
                Fusion_HitBox2Pointer = s.SerializePointer(Fusion_HitBox2Pointer, name: nameof(Fusion_HitBox2Pointer));
                Fusion_HitBox3Pointer = s.SerializePointer(Fusion_HitBox3Pointer, name: nameof(Fusion_HitBox3Pointer));
                Fusion_Pointer_24 = s.SerializePointer(Fusion_Pointer_24, name: nameof(Fusion_Pointer_24));
                Fusion_Pointer_28 = s.SerializePointer(Fusion_Pointer_28, name: nameof(Fusion_Pointer_28));

                TilePositions = s.DoAt(TilePositionsPointer, () => s.SerializeObjectArray<TilePosition>(TilePositions, TilesCount, name: nameof(TilePositions)));

                if (TilePositions != null)
                    Fusion_TileSet = s.DoAt(Fusion_TileSetPointer, () => s.SerializeArray<byte>(Fusion_TileSet, TilePositions.Select(x => GBAVV_BaseManager.TileShapes[x.ShapeIndex]).Sum(x => x.x * x.y / 2), name: nameof(Fusion_TileSet)));

                Fusion_HitBox1 = s.DoAt(Fusion_HitBox1Pointer, () => s.SerializeObject<GBAVV_AnimationRect>(Fusion_HitBox1, name: nameof(Fusion_HitBox1)));
                Fusion_HitBox2 = s.DoAt(Fusion_HitBox2Pointer, () => s.SerializeObject<GBAVV_AnimationRect>(Fusion_HitBox2, name: nameof(Fusion_HitBox2)));
                Fusion_HitBox3 = s.DoAt(Fusion_HitBox3Pointer, () => s.SerializeObject<GBAVV_AnimationRect>(Fusion_HitBox3, name: nameof(Fusion_HitBox3)));
            }
            else if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart_NGage)
            {
                NitroKart_NGage_ImageDataPointer = s.SerializePointer(NitroKart_NGage_ImageDataPointer, name: nameof(NitroKart_NGage_ImageDataPointer));
                RenderBox = s.SerializeObject<GBAVV_AnimationRect>(RenderBox, name: nameof(RenderBox));

                NitroKart_NGage_ImageData = s.DoAt(NitroKart_NGage_ImageDataPointer, () => s.SerializeArray<byte>(NitroKart_NGage_ImageData, NitroKart_NGage_Width * NitroKart_NGage_Height, name: nameof(NitroKart_NGage_ImageData)));
            }
            else
            {
                TilePositionsPointer = s.SerializePointer(TilePositionsPointer, name: nameof(TilePositionsPointer));
                TileShapesPointer = s.SerializePointer(TileShapesPointer, name: nameof(TileShapesPointer));
                TileOffset = s.Serialize<UInt24>(TileOffset, name: nameof(TileOffset));
                TilesCount = s.Serialize<byte>(TilesCount, name: nameof(TilesCount));

                if (s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashNitroKart || s.GetR1Settings().EngineVersion == EngineVersion.GBAVV_KidsNextDoorOperationSODA)
                {
                    RenderBox = s.SerializeObject<GBAVV_AnimationRect>(RenderBox, name: nameof(RenderBox));
                    NitroKart_Pointer_10 = s.SerializePointer(NitroKart_Pointer_10, name: nameof(NitroKart_Pointer_10));

                    // TODO: GBAVV_KidsNextDoorOperationSODA has additional pointers after this - hitboxes (like in Fusion)?
                }

                TilePositions = s.DoAt(TilePositionsPointer, () => s.SerializeObjectArray<TilePosition>(TilePositions, TilesCount, name: nameof(TilePositions)));
                TileShapes = s.DoAt(TileShapesPointer, () => s.SerializeObjectArray<TileShape>(TileShapes, TilesCount, name: nameof(TileShapes)));
            }
        }

        public class TilePosition : BinarySerializable
        {
            public short XPos { get; set; }
            public short YPos { get; set; }
            public int ShapeIndex { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                XPos = s.Serialize<short>(XPos, name: nameof(XPos));
                YPos = s.Serialize<short>(YPos, name: nameof(YPos));

                if (s.GetR1Settings().EngineVersion >= EngineVersion.GBAVV_CrashFusion && s.GetR1Settings().EngineVersion != EngineVersion.GBAVV_KidsNextDoorOperationSODA)
                    ShapeIndex = s.Serialize<int>(ShapeIndex, name: nameof(ShapeIndex));
            }
        }

        public class TileShape : BinarySerializable
        {
            public byte ShapeIndex { get; set; }
            public byte Unknown { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                s.SerializeBitValues<byte>(bitFunc =>
                {
                    ShapeIndex = (byte)bitFunc(ShapeIndex, 4, name: nameof(ShapeIndex));
                    Unknown = (byte)bitFunc(Unknown, 4, name: nameof(Unknown));
                });
            }
        }
    }
}