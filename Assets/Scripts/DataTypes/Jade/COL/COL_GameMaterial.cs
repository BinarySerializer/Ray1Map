using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class COL_GameMaterial : Jade_File {
		public override string Export_Extension => "gam";
		public uint Count { get; set; }
        public uint[] UInts { get; set; }
        public Material[] Materials { get; set; }
        public PhoenixStruct Phoenix { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Count = s.Serialize<uint>(Count, name: nameof(Count));
            UInts = s.SerializeArray<uint>(UInts, Count, name: nameof(UInts));
            Materials = s.SerializeObjectArray<Material>(Materials, Count, name: nameof(Materials));
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PetzHorseClub) && Materials?.Length >= 1 && Materials[Count-1].DummyByte > 10) {
				Phoenix = s.SerializeObject<PhoenixStruct>(Phoenix, name: nameof(Phoenix));
			}

        }
		public class Material : BinarySerializable {
            public uint CustomBits { get; set; }
            public float Slide { get; set; }
            public float Rebound { get; set; }
            public byte Sound { get; set; }
            public byte DummyByte { get; set; }
            public ushort DummyUShort { get; set; }

            public string Name { get; set; }
            public uint UInt_Editor_0 { get; set; }
            public uint UInt_Editor_1 { get; set; }
            public uint UInt_Editor_2 { get; set; }
            public uint UInt_Editor_3 { get; set; }

            public override void SerializeImpl(SerializerObject s) {
				CustomBits = s.Serialize<uint>(CustomBits, name: nameof(CustomBits));
                Slide = s.Serialize<float>(Slide, name: nameof(Slide));
                Rebound = s.Serialize<float>(Rebound, name: nameof(Rebound));
                Sound = s.Serialize<byte>(Sound, name: nameof(Sound));
                DummyByte = s.Serialize<byte>(DummyByte, name: nameof(DummyByte));
                DummyUShort = s.Serialize<ushort>(DummyUShort, name: nameof(DummyUShort));

                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
                if (!Loader.IsBinaryData && DummyByte > 0) {
                    Name = s.SerializeString(Name, 64, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
                    UInt_Editor_0 = s.Serialize<uint>(UInt_Editor_0, name: nameof(UInt_Editor_0));
                    if (DummyByte >= 2) UInt_Editor_1 = s.Serialize<uint>(UInt_Editor_1, name: nameof(UInt_Editor_1));
                    UInt_Editor_2 = s.Serialize<uint>(UInt_Editor_2, name: nameof(UInt_Editor_2));
                    if (DummyByte >= 3) UInt_Editor_3 = s.Serialize<uint>(UInt_Editor_3, name: nameof(UInt_Editor_3));
                }
            }
		}
		public class PhoenixStruct : BinarySerializable {
            public Element[] Elements { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Elements = s.SerializeObjectArray<Element>(Elements, 8, name: nameof(Elements));
			}

			public class Element : BinarySerializable {
                public uint[] UInts { get; set; }
				public override void SerializeImpl(SerializerObject s) {
					UInts = s.SerializeArray<uint>(UInts, 10, name: nameof(UInts));
				}
			}
		}
	}
}