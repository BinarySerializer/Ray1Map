using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class WOR_World : Jade_File {
		public Jade_FileType Type { get; set; }
		public uint Version { get; set; }
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
		public Jade_Reference<GRID_WorldGrid> Grid0 { get; set; }
		public Jade_Reference<GRID_WorldGrid> Grid1 { get; set; }
		public Jade_Reference<WOR_GameObjectGroup> GameObjects { get; set; }
		public Jade_Reference<WAY_AllNetworks> Networks { get; set; }
		public Jade_Key TextKey { get; set; } // TODO: Change this to a reference, push this to the loader but load as separate bin
		public UnknownStruct[] UnknownStructs { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
			if (Type.Type != Jade_FileType.FileType.WOR_World)
				throw new Exception($"Parsing failed: File at {Offset} was not of type {Jade_FileType.FileType.WOR_World}");

			Version = s.Serialize<uint>(Version, name: nameof(Version));
			UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
			UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
			Name = s.SerializeString(Name, length: 60, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
			if(!Loader.IsBinaryData) UInt_AfterName = s.Serialize<uint>(UInt_AfterName, name: nameof(UInt_AfterName));
			Matrix = s.SerializeObject<Jade_Matrix>(Matrix, name: nameof(Matrix));
			Float_90 = s.Serialize<float>(Float_90, name: nameof(Float_90));
			UInt_94 = s.Serialize<uint>(UInt_94, name: nameof(UInt_94));
			UInt_98 = s.Serialize<uint>(UInt_98, name: nameof(UInt_98));
			if (Version >= 5) {
				UInt_9C_Version5 = s.Serialize<uint>(UInt_9C_Version5, name: nameof(UInt_9C_Version5));
			}
			UInt_A0 = s.Serialize<uint>(UInt_A0, name: nameof(UInt_A0));
			if (!Loader.IsBinaryData) Bytes_A4 = s.SerializeArray<byte>(Bytes_A4, 44, name: nameof(Bytes_A4));
			Grid0 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid0, name: nameof(Grid0))?.Resolve();
			Grid1 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid1, name: nameof(Grid1))?.Resolve();
			GameObjects = s.SerializeObject<Jade_Reference<WOR_GameObjectGroup>>(GameObjects, name: nameof(GameObjects))?.Resolve();
			Networks = s.SerializeObject<Jade_Reference<WAY_AllNetworks>>(Networks, name: nameof(Networks))?.Resolve();
			TextKey = s.SerializeObject<Jade_Key>(TextKey, name: nameof(TextKey));
			if (Version > 3) {
				UnknownStructs = s.SerializeObjectArray<UnknownStruct>(UnknownStructs, 64, name: nameof(UnknownStructs));
			}
		}

		public class UnknownStruct : R1Serializable {
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

			public class InnerStruct : R1Serializable {
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
	}
}
