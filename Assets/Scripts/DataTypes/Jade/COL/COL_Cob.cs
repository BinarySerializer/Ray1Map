using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class COL_Cob : Jade_File 
    {
        public Jade_Reference<COL_GameMaterial> Material { get; set; }
        public byte Type { get; set; } // 1, 2, 3 or 5
        public byte Flags { get; set; }
        public uint Uint_06 { get; set; }

        public COL_Box Shape_Box { get; set; } // Type 1
        public COL_Sphere Shape_Sphere { get; set; } // Type 2

        // Type 3
        public Jade_Vector Type3_Vector { get; set; }
        public float Type3_Float_04 { get; set; }
        public float Type3_Float_08 { get; set; }

        public COL_IndexedTriangles Shape_IndexedTriangles { get; set; } // Type 5

        public override void SerializeImpl(SerializerObject s) 
        {
            Material = s.SerializeObject<Jade_Reference<COL_GameMaterial>>(Material, name: nameof(Material))?.Resolve();
            Type = s.Serialize(Type, name: nameof(Type));
            Flags = s.Serialize(Flags, name: nameof(Flags));

            if (Type != 1 && Type != 2 && Type != 3 && Type != 5)
                return;

            if (Type != 5)
                Uint_06 = s.Serialize<uint>(Uint_06, name: nameof(Uint_06));

            switch (Type)
            {
                case 1:
                    Shape_Box = s.SerializeObject<COL_Box>(Shape_Box, name: nameof(Shape_Box));
                    break;

                case 2:
                    Shape_Sphere = s.SerializeObject<COL_Sphere>(Shape_Sphere, name: nameof(Shape_Sphere));
                    break;

                case 3:
                    Type3_Vector = s.SerializeObject<Jade_Vector>(Type3_Vector, name: nameof(Type3_Vector));
                    Type3_Float_04 = s.Serialize<float>(Type3_Float_04, name: nameof(Type3_Float_04));
                    Type3_Float_08 = s.Serialize<float>(Type3_Float_08, name: nameof(Type3_Float_08));
                    break;

                case 5:
                    Shape_IndexedTriangles = s.SerializeObject<COL_IndexedTriangles>(Shape_IndexedTriangles,
                        onPreSerialize: it => it.Flags = Flags,
                        name: nameof(Shape_IndexedTriangles));

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