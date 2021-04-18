using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class WOR_World : Jade_File {
		public Jade_FileType FileType { get; set; }
		public uint Type { get; set; }
		public uint UInt_08 { get; set; }
		public uint UInt_0C { get; set; } // a key?
		public string Name { get; set; }
		public uint UInt_AfterName { get; set; }
		public Jade_Matrix Matrix { get; set; }
		public float Float_90 { get; set; }
		public uint UInt_94 { get; set; } // a key?
		public uint UInt_98 { get; set; } // a key?
		public uint UInt_9C_Version5 { get; set; }
		public uint UInt_A0 { get; set; }
		public byte[] Bytes_A4 { get; set; }
		public XenonStruct Xenon { get; set; }
		public Jade_Reference<GRID_WorldGrid> Grid0 { get; set; }
		public Jade_Reference<GRID_WorldGrid> Grid1 { get; set; }
		public Jade_Reference<WOR_GameObjectGroup> GameObjects { get; set; }
		public Jade_Reference<WAY_AllNetworks> Networks { get; set; }
		public Jade_TextReference Text { get; set; }
		public UnknownStruct[] UnknownStructs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			FileType = s.SerializeObject<Jade_FileType>(FileType, name: nameof(FileType));
			if (FileType.Type != Jade_FileType.FileType.WOR_World)
				throw new Exception($"Parsing failed: File at {Offset} was not of type {Jade_FileType.FileType.WOR_World}");

			Type = s.Serialize<uint>(Type, name: nameof(Type));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
			Name = s.SerializeString(Name, length: 60, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			if(!Loader.IsBinaryData) UInt_AfterName = s.Serialize<uint>(UInt_AfterName, name: nameof(UInt_AfterName));
			Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
			Float_90 = s.Serialize<float>(Float_90, name: nameof(Float_90));
			UInt_94 = s.Serialize<uint>(UInt_94, name: nameof(UInt_94));
			UInt_98 = s.Serialize<uint>(UInt_98, name: nameof(UInt_98));
			if (Type >= 5) {
				UInt_9C_Version5 = s.Serialize<uint>(UInt_9C_Version5, name: nameof(UInt_9C_Version5));
			}
			UInt_A0 = s.Serialize<uint>(UInt_A0, name: nameof(UInt_A0));
			if (s.GetR1Settings().EngineVersion == EngineVersion.Jade_RRR_Xbox360) {
				Xenon = s.SerializeObject<XenonStruct>(Xenon, onPreSerialize: x => x.Type = Type, name: nameof(Xenon));
			} else {
				if (!Loader.IsBinaryData) Bytes_A4 = s.SerializeArray<byte>(Bytes_A4, 44, name: nameof(Bytes_A4));
			}
			Grid0 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid0, name: nameof(Grid0))?.Resolve();
			Grid1 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid1, name: nameof(Grid1))?.Resolve();
			GameObjects = s.SerializeObject<Jade_Reference<WOR_GameObjectGroup>>(GameObjects, name: nameof(GameObjects))
				?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontCache);
			Networks = s.SerializeObject<Jade_Reference<WAY_AllNetworks>>(Networks, name: nameof(Networks))?.Resolve();
			Text = s.SerializeObject<Jade_TextReference>(Text, name: nameof(Text))?.Resolve();
			if (Type > 3) {
				UnknownStructs = s.SerializeObjectArray<UnknownStruct>(UnknownStructs, 64, name: nameof(UnknownStructs));
			}
		}

		public class UnknownStruct : BinarySerializable {
			public uint UInt_00 { get; set; }
			public byte[] Bytes_04 { get; set; }
			public byte[] Bytes_14 { get; set; }
			public byte[] Bytes_24 { get; set; }
			public InnerStruct[] InnerStructs { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				Bytes_04 = s.SerializeArray<byte>(Bytes_04, 0x10, name: nameof(Bytes_04));
				Bytes_14 = s.SerializeArray<byte>(Bytes_14, 0x10, name: nameof(Bytes_14));
				if(!Loader.IsBinaryData) Bytes_24 = s.SerializeArray<byte>(Bytes_24, 0x40, name: nameof(Bytes_24));
				InnerStructs = s.SerializeObjectArray<InnerStruct>(InnerStructs, 16, name: nameof(InnerStructs));
			}

			public class InnerStruct : BinarySerializable {
				public short Short_00 { get; set; }
				public byte Byte_02 { get; set; }
				public byte Byte_03 { get; set; }
				public Jade_Vector Vector_04 { get; set; }
				public Jade_Vector Vector_10 { get; set; }
				public Jade_Vector Vector_1C { get; set; }
				public Jade_Vector Vector_28 { get; set; }
				public byte[] Bytes_34 { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
					Short_00 = s.Serialize<short>(Short_00, name: nameof(Short_00));
					Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
					Byte_03 = s.Serialize<byte>(Byte_03, name: nameof(Byte_03));
					Vector_04 = s.SerializeObject<Jade_Vector>(Vector_04, name: nameof(Vector_04));
					Vector_10 = s.SerializeObject<Jade_Vector>(Vector_10, name: nameof(Vector_10));
					Vector_1C = s.SerializeObject<Jade_Vector>(Vector_1C, name: nameof(Vector_1C));
					Vector_28 = s.SerializeObject<Jade_Vector>(Vector_28, name: nameof(Vector_28));
					if (!Loader.IsBinaryData) Bytes_34 = s.SerializeArray<byte>(Bytes_34, 0x40, name: nameof(Bytes_34));
				}
			}
		}

		public class XenonStruct : BinarySerializable {
			public uint Type { get; set; } // Set in onPreSerialize

			public float Float_00 { get; set; }
			public float Float_04 { get; set; }
			public float Float_08 { get; set; }
			public int Int_0C { get; set; }
			public float Float_10 { get; set; }

			public float Float_14 { get; set; }
			public float Float_18 { get; set; }
			public float Float_1C { get; set; }
			public float Float_20 { get; set; }
			public float Float_24 { get; set; }

			public float Type6_Float_00 { get; set; }
			public float Type6_Float_04 { get; set; }
			public float Type6_Float_08 { get; set; }
			public float Type6_Float_0C { get; set; }

			public float Type7_Float_00 { get; set; }
			public float Type7_Float_04 { get; set; }

			public uint Type8_UInt_00 { get; set; } // A color? RGBA (0xFF808080)
			public uint Type8_UInt_04 { get; set; } // same

			public float Type9_Float_00 { get; set; }
			public int Type9_Int_04 { get; set; }

			public float Type10_Float_00 { get; set; }
			public float Type10_Float_04 { get; set; }

			public uint Type11_UInt_00 { get; set; }
			public float Type11_Float_04 { get; set; }
			public float Type11_Float_08 { get; set; }
			public uint Type11_UInt_0C { get; set; }
			public float Type11_Float_10 { get; set; }
			public float Type11_Float_14 { get; set; }

			public float Type12_Float_00 { get; set; }
			public float Type12_Float_04 { get; set; }
			public float Type12_Float_08 { get; set; }

			public float Type13_Float_00 { get; set; }
			public float Type13_Float_04 { get; set; }
			public float Type13_Float_08 { get; set; }
			public float Type13_Float_0C { get; set; }
			public float Type13_Float_10 { get; set; }

			public Jade_Reference<OBJ_GameObject> GameObject { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Float_00 = s.Serialize<float>(Float_00, name: nameof(Float_00));
				Float_04 = s.Serialize<float>(Float_04, name: nameof(Float_04));
				Float_08 = s.Serialize<float>(Float_08, name: nameof(Float_08));
				Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
				Float_10 = s.Serialize<float>(Float_10, name: nameof(Float_10));
				Float_14 = s.Serialize<float>(Float_14, name: nameof(Float_14));
				Float_18 = s.Serialize<float>(Float_18, name: nameof(Float_18));
				Float_1C = s.Serialize<float>(Float_1C, name: nameof(Float_1C));
				Float_20 = s.Serialize<float>(Float_20, name: nameof(Float_20));
				Float_24 = s.Serialize<float>(Float_24, name: nameof(Float_24));
				if (Type >= 6) {
					Type6_Float_00 = s.Serialize<float>(Type6_Float_00, name: nameof(Type6_Float_00));
					Type6_Float_04 = s.Serialize<float>(Type6_Float_04, name: nameof(Type6_Float_04));
					Type6_Float_08 = s.Serialize<float>(Type6_Float_08, name: nameof(Type6_Float_08));
					Type6_Float_0C = s.Serialize<float>(Type6_Float_0C, name: nameof(Type6_Float_0C));
				}
				if (Type >= 7) {
					Type7_Float_00 = s.Serialize<float>(Type7_Float_00, name: nameof(Type7_Float_00));
					Type7_Float_04 = s.Serialize<float>(Type7_Float_04, name: nameof(Type7_Float_04));
				}
				if (Type >= 8) {
					Type8_UInt_00 = s.Serialize<uint>(Type8_UInt_00, name: nameof(Type8_UInt_00));
					Type8_UInt_04 = s.Serialize<uint>(Type8_UInt_04, name: nameof(Type8_UInt_04));
				}
				if (Type >= 9) {
					Type9_Float_00 = s.Serialize<float>(Type9_Float_00, name: nameof(Type9_Float_00));
					Type9_Int_04 = s.Serialize<int>(Type9_Int_04, name: nameof(Type9_Int_04));
				}
				if (Type >= 10) {
					Type10_Float_00 = s.Serialize<float>(Type10_Float_00, name: nameof(Type10_Float_00));
					Type10_Float_04 = s.Serialize<float>(Type10_Float_04, name: nameof(Type10_Float_04));
				}
				if (Type >= 11) {
					Type11_UInt_00 = s.Serialize<uint>(Type11_UInt_00, name: nameof(Type11_UInt_00));
					Type11_Float_04 = s.Serialize<float>(Type11_Float_04, name: nameof(Type11_Float_04));
					Type11_Float_08 = s.Serialize<float>(Type11_Float_08, name: nameof(Type11_Float_08));
					Type11_UInt_0C = s.Serialize<uint>(Type11_UInt_0C, name: nameof(Type11_UInt_0C));
					Type11_Float_10 = s.Serialize<float>(Type11_Float_10, name: nameof(Type11_Float_10));
					Type11_Float_14 = s.Serialize<float>(Type11_Float_14, name: nameof(Type11_Float_14));
				}
				if (Type == 12) {
					Type12_Float_00 = s.Serialize<float>(Type12_Float_00, name: nameof(Type12_Float_00));
					Type12_Float_04 = s.Serialize<float>(Type12_Float_04, name: nameof(Type12_Float_04));
					Type12_Float_08 = s.Serialize<float>(Type12_Float_08, name: nameof(Type12_Float_08));
				}
				if (Type >= 13) {
					Type13_Float_00 = s.Serialize<float>(Type13_Float_00, name: nameof(Type13_Float_00));
					Type13_Float_04 = s.Serialize<float>(Type13_Float_04, name: nameof(Type13_Float_04));
					Type13_Float_08 = s.Serialize<float>(Type13_Float_08, name: nameof(Type13_Float_08));
					Type13_Float_0C = s.Serialize<float>(Type13_Float_0C, name: nameof(Type13_Float_0C));
					Type13_Float_10 = s.Serialize<float>(Type13_Float_10, name: nameof(Type13_Float_10));
				}
				GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
			}
		}
	}
}
