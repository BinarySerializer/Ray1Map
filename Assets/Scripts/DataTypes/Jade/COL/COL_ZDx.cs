using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_ZDx : BinarySerializable {
        public bool IsInstance { get; set; } // Set in OnPreSerialize

        public byte Flag { get; set; }
        public byte Type { get; set; } // Determines shape
        public byte BoneIndex { get; set; }
        public byte Design { get; set; }
        public byte Byte_04_Editor { get; set; }
        public uint NameLength { get; set; }
        public string Name { get; set; }
        public byte Name_Terminator { get; set; }

        // Copied from COL_Cob-
        public COL_Box Shape_Box { get; set; } // Type 1
        public COL_Sphere Shape_Sphere { get; set; } // Type 2

        // Type 3
        public Jade_Vector Type3_Vector { get; set; }
        public float Type3_Float_04 { get; set; }
        public float Type3_Float_08 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Flag = s.Serialize<byte>(Flag, name: nameof(Flag));
            Type = s.Serialize<byte>(Type, name: nameof(Type));
            BoneIndex = s.Serialize<byte>(BoneIndex, name: nameof(BoneIndex));
            Design = s.Serialize<byte>(Design, name: nameof(Design));
            if (IsInstance && !Loader.IsBinaryData) Byte_04_Editor = s.Serialize<byte>(Byte_04_Editor, name: nameof(Byte_04_Editor));
            NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
            if (!Loader.IsBinaryData) {
                Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
                Name_Terminator = s.Serialize<byte>(Name_Terminator, name: nameof(Name_Terminator));
            }

            switch (Type) {
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
            }
        }
    }
}