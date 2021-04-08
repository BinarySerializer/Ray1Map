using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_GameMaterial : Jade_File {
        public uint Count { get; set; }
        public uint[] UInts { get; set; }
        public Material[] Materials { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Count = s.Serialize<uint>(Count, name: nameof(Count));
            UInts = s.SerializeArray<uint>(UInts, Count, name: nameof(UInts));
            Materials = s.SerializeObjectArray<Material>(Materials, Count, name: nameof(Materials));

        }
		public class Material : BinarySerializable {
            public uint Flags { get; set; }
            public float Slide { get; set; }
            public float Rebound { get; set; }
            public byte Sound { get; set; }
            public byte ByteUnknown { get; set; }
            public ushort ID { get; set; }

            public string Name { get; set; }
            public uint UInt_Editor_0 { get; set; }
            public uint UInt_Editor_1 { get; set; }
            public uint UInt_Editor_2 { get; set; }
            public uint UInt_Editor_3 { get; set; }

            public override void SerializeImpl(SerializerObject s) {
				Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
                Slide = s.Serialize<float>(Slide, name: nameof(Slide));
                Rebound = s.Serialize<float>(Rebound, name: nameof(Rebound));
                Sound = s.Serialize<byte>(Sound, name: nameof(Sound));
                ByteUnknown = s.Serialize<byte>(ByteUnknown, name: nameof(ByteUnknown));
                ID = s.Serialize<ushort>(ID, name: nameof(ID));

                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                if (!Loader.IsBinaryData && ByteUnknown > 0) {
                    Name = s.SerializeString(Name, 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
                    UInt_Editor_0 = s.Serialize<uint>(UInt_Editor_0, name: nameof(UInt_Editor_0));
                    if (ByteUnknown >= 2) UInt_Editor_1 = s.Serialize<uint>(UInt_Editor_1, name: nameof(UInt_Editor_1));
                    UInt_Editor_2 = s.Serialize<uint>(UInt_Editor_2, name: nameof(UInt_Editor_2));
                    if (ByteUnknown >= 3) UInt_Editor_3 = s.Serialize<uint>(UInt_Editor_3, name: nameof(UInt_Editor_3));
                }
            }
		}
	}
}