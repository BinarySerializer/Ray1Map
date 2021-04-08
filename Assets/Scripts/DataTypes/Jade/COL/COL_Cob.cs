using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class COL_Cob : Jade_File 
    {
        public Jade_Reference<COL_GameMaterial> Material { get; set; }
        public byte CobType { get; set; } // 1, 2, 3 or 5
        public byte Flags { get; set; }
        public uint Uint_06 { get; set; }

        // Type 1
        public Jade_Vector Type1_Vector_00 { get; set; }
        public Jade_Vector Type1_Vector_04 { get; set; }

        // Type 2
        public Jade_Vector Type2_Vector { get; set; }
        public float Type2_Float_04 { get; set; }

        // Type 3
        public Jade_Vector Type3_Vector { get; set; }
        public float Type3_Float_04 { get; set; }
        public float Type3_Float_08 { get; set; }

        // Type 5
        public uint Type5_Vectors_04_Count { get; set; }
        public Jade_Vector[] Type5_Vectors_04 { get; set; }
        public uint Type5_Vectors_08_Count { get; set; }
        public Jade_Vector[] Type5_Vectors_08 { get; set; }
        public uint Type5_UnkStructs_Count { get; set; }
        public UnkStruct[] Type5_UnkStructs { get; set; }
        public ColMapPoint[][] Type5_AdditionalColMapPoints { get; set; }
        public GEO_GeometricObject_CollisionData Type5_CollisionData { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            Material = s.SerializeObject<Jade_Reference<COL_GameMaterial>>(Material, name: nameof(Material))?.Resolve();
            CobType = s.Serialize(CobType, name: nameof(CobType));
            Flags = s.Serialize(Flags, name: nameof(Flags));

            if (CobType != 1 && CobType != 2 && CobType != 3 && CobType != 5)
                return;

            if (CobType != 5)
                Uint_06 = s.Serialize<uint>(Uint_06, name: nameof(Uint_06));

            switch (CobType)
            {
                case 1:
                    Type1_Vector_00 = s.SerializeObject<Jade_Vector>(Type1_Vector_00, name: nameof(Type1_Vector_00));
                    Type1_Vector_04 = s.SerializeObject<Jade_Vector>(Type1_Vector_04, name: nameof(Type1_Vector_04));
                    break;

                case 2:
                    Type2_Vector = s.SerializeObject<Jade_Vector>(Type2_Vector, name: nameof(Type2_Vector));
                    Type2_Float_04 = s.Serialize<float>(Type2_Float_04, name: nameof(Type2_Float_04));
                    break;

                case 3:
                    Type3_Vector = s.SerializeObject<Jade_Vector>(Type3_Vector, name: nameof(Type3_Vector));
                    Type3_Float_04 = s.Serialize<float>(Type3_Float_04, name: nameof(Type3_Float_04));
                    Type3_Float_08 = s.Serialize<float>(Type3_Float_08, name: nameof(Type3_Float_08));
                    break;

                case 5:
                    Type5_Vectors_04_Count = s.Serialize<uint>(Type5_Vectors_04_Count, name: nameof(Type5_Vectors_04_Count));
                    Type5_Vectors_04 = s.SerializeObjectArray<Jade_Vector>(Type5_Vectors_04, Type5_Vectors_04_Count, name: nameof(Type5_Vectors_04));

                    Type5_Vectors_08_Count = s.Serialize<uint>(Type5_Vectors_08_Count, name: nameof(Type5_Vectors_08_Count));
                    Type5_Vectors_08 = s.SerializeObjectArray<Jade_Vector>(Type5_Vectors_08, Type5_Vectors_08_Count, name: nameof(Type5_Vectors_08));

                    Type5_UnkStructs_Count = s.Serialize<uint>(Type5_UnkStructs_Count, name: nameof(Type5_UnkStructs_Count));
                    Type5_UnkStructs = s.SerializeObjectArray<UnkStruct>(Type5_UnkStructs, Type5_UnkStructs_Count, name: nameof(Type5_UnkStructs));

                    if ((Flags & 0x80) != 0 && Loader.IsBinaryData)
                    {
                        if (Type5_AdditionalColMapPoints == null)
                            Type5_AdditionalColMapPoints = new ColMapPoint[Type5_UnkStructs_Count][];

                        for (int i = 0; i < Type5_AdditionalColMapPoints.Length; i++)
                            Type5_AdditionalColMapPoints[i] = s.SerializeObjectArray(Type5_AdditionalColMapPoints[i], Type5_UnkStructs[i].ColMapPointsCount, name: $"{nameof(Type5_AdditionalColMapPoints)}[{i}]");

                        if ((Flags & 8) == 8) {
                            Type5_CollisionData = s.SerializeObject<GEO_GeometricObject_CollisionData>(Type5_CollisionData, name: nameof(Type5_CollisionData));
                        }
                    }

                    break;
            }
        }

        public class UnkStruct : BinarySerializable
        {
            public ushort ColMapPointsCount { get; set; }
            public byte Byte_02 { get; set; }
            public byte Byte_03 { get; set; }
            public uint Uint_04 { get; set; }
            public ColMapPoint[] ColMapPoints { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                ColMapPointsCount = s.Serialize<ushort>(ColMapPointsCount, name: nameof(ColMapPointsCount));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
                Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
                ColMapPoints = s.SerializeObjectArray<ColMapPoint>(ColMapPoints, ColMapPointsCount, name: nameof(ColMapPoints));
            }
        }

        public class ColMapPoint : BinarySerializable
        {
            public short Short_00 { get; set; }
            public short Short_02 { get; set; }
            public short Short_04 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
                Short_02 = s.Serialize<short>(Short_02, name: nameof(Short_02));
                Short_04 = s.Serialize<short>(Short_04, name: nameof(Short_04));
            }
        }
    }
}