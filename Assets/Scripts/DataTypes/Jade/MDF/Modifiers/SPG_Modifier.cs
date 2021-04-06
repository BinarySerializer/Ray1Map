using BinarySerializer;

namespace R1Engine.Jade {
	public class SPG_Modifier : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Type { get; set; }
		public float Float_04 { get; set; }
		public float Float_08 { get; set; }
		public uint UInt_0C { get; set; }

		public float Type1_Float_0 { get; set; }
		public float Type1_Float_1 { get; set; }
		public uint Type1_UInt { get; set; }
		public uint Type2_UInt { get; set; }
		public float Type3_Float { get; set; }
		public byte[][] Type4_Bytes { get; set; }
		public Type5_Struct[] Type5_Structs { get; set; }
		public float Type6_Float { get; set; }
		public uint Type7_UInt { get; set; }
		public float Type8_Float_0 { get; set; }
		public float Type8_Float_1 { get; set; }
		public uint Type8_UInt { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
			Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
			UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
			if (Type >= 1) {
				Type1_Float_0 = s.Serialize<float>(Type1_Float_0, name: nameof(Type1_Float_0));
				Type1_Float_1 = s.Serialize<float>(Type1_Float_1, name: nameof(Type1_Float_1));
				Type1_UInt = s.Serialize<uint>(Type1_UInt, name: nameof(Type1_UInt));
			}
			if (Type >= 2) Type2_UInt = s.Serialize<uint>(Type2_UInt, name: nameof(Type2_UInt));
			if (Type >= 3) Type3_Float = s.Serialize<float>(Type3_Float, name: nameof(Type3_Float));
			if (Type >= 4) {
				if(Type4_Bytes == null) Type4_Bytes = new byte[4][];
				for (int i = 0; i < Type4_Bytes.Length; i++) {
					Type4_Bytes[i] = s.SerializeArray<byte>(Type4_Bytes[i], 8, name: $"{nameof(Type4_Bytes)}[{i}]");
				}
			}
			if (Type >= 5) Type5_Structs = s.SerializeObjectArray<Type5_Struct>(Type5_Structs, 4, name: nameof(Type5_Structs));
			if (Type >= 6) Type6_Float = s.Serialize<float>(Type6_Float, name: nameof(Type6_Float));
			if (Type >= 7) Type7_UInt = s.Serialize<uint>(Type7_UInt, name: nameof(Type7_UInt));
			if (Type >= 8) {
				Type8_Float_0 = s.Serialize<float>(Type8_Float_0, name: nameof(Type8_Float_0));
				Type8_Float_1 = s.Serialize<float>(Type8_Float_1, name: nameof(Type8_Float_1));
				Type8_UInt = s.Serialize<uint>(Type8_UInt, name: nameof(Type8_UInt));
			}
		}

		public class Type5_Struct : BinarySerializable {
			public uint UInt_00 { get; set; }
			public float Float_04 { get; set; }
			public float Float_08 { get; set; }
			public float Float_0C { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
				Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
				Float_0C = s.Serialize<float>(Float_0C, name: nameof(Float_0C));
			}
		}
	}
}
