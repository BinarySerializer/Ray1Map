using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_ColSetStruct : BinarySerializable {
        public bool IsInstance { get; set; } // Set in OnPreSerialize

        public byte Byte_00 { get; set; }
        public byte CobType { get; set; }
        public byte Byte_02 { get; set; }
        public byte Byte_03 { get; set; }
        public byte Byte_04_Editor { get; set; }
        public uint NameLength { get; set; }
        public string Name { get; set; }
        public byte Name_Terminator { get; set; }

        // Copied from COL_Cob
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

        public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Byte_00 = s.Serialize<byte>(Byte_00, name: nameof(Byte_00));
            CobType = s.Serialize<byte>(CobType, name: nameof(CobType));
            Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
            Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
            if (IsInstance && !Loader.IsBinaryData) Byte_04_Editor = s.Serialize<byte>(Byte_04_Editor, name: nameof(Byte_04_Editor));
            NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
            if (!Loader.IsBinaryData) {
                Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
                Name_Terminator = s.Serialize<byte>(Name_Terminator, name: nameof(Name_Terminator));
            }

            switch (CobType) {
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
            }
        }
    }
}