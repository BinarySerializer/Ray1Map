using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_ZDx : BinarySerializable {
        public bool IsInstance { get; set; } // Set in OnPreSerialize

        public byte Flag { get; set; }
        public COL_ZoneShape Type { get; set; } // Determines shape
        public byte BoneIndex { get; set; }
        public byte Design { get; set; }
        public byte Byte_04_Editor { get; set; }
        public uint NameLength { get; set; }
        public string Name { get; set; }
        public byte Name_Terminator { get; set; }

        // Copied from COL_Cob
        public COL_Box Shape_Box { get; set; } // Type 1
        public COL_Sphere Shape_Sphere { get; set; } // Type 2
        public COL_Cylinder Shape_Cylinder { get; set; } // Type 3

        public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Flag = s.Serialize<byte>(Flag, name: nameof(Flag));
            Type = s.Serialize<COL_ZoneShape>(Type, name: nameof(Type));
            BoneIndex = s.Serialize<byte>(BoneIndex, name: nameof(BoneIndex));
            Design = s.Serialize<byte>(Design, name: nameof(Design));
            if (IsInstance && !Loader.IsBinaryData) Byte_04_Editor = s.Serialize<byte>(Byte_04_Editor, name: nameof(Byte_04_Editor));
            NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
            if (!Loader.IsBinaryData) {
                Name = s.SerializeString(Name, NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
                Name_Terminator = s.Serialize<byte>(Name_Terminator, name: nameof(Name_Terminator));
            }

            switch (Type) {
                case COL_ZoneShape.Box:
                    Shape_Box = s.SerializeObject<COL_Box>(Shape_Box, name: nameof(Shape_Box));
                    break;

                case COL_ZoneShape.Sphere:
                    Shape_Sphere = s.SerializeObject<COL_Sphere>(Shape_Sphere, name: nameof(Shape_Sphere));
                    break;

                case COL_ZoneShape.Cylinder:
                    Shape_Cylinder = s.SerializeObject<COL_Cylinder>(Shape_Cylinder, name: nameof(Shape_Cylinder));
                    break;
            }
        }
    }
}