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
			Grid0 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid0, name: nameof(Grid0));
			Grid0.Resolve();
			Grid1 = s.SerializeObject<Jade_Reference<GRID_WorldGrid>>(Grid1, name: nameof(Grid1));
			Grid1.Resolve();
			GameObjects = s.SerializeObject<Jade_Reference<WOR_GameObjectGroup>>(GameObjects, name: nameof(GameObjects));
			GameObjects.Resolve();
			Networks = s.SerializeObject<Jade_Reference<WAY_AllNetworks>>(Networks, name: nameof(Networks));
			Networks.Resolve();
			TextKey = s.SerializeObject<Jade_Key>(TextKey, name: nameof(TextKey));
			if (Version > 3) {
				UnknownStructs = s.SerializeObjectArray<UnknownStruct>(UnknownStructs, 64, name: nameof(UnknownStructs));
			}
		}

		public class UnknownStruct : R1Serializable {
			public override void SerializeImpl(SerializerObject s) {
				throw new NotImplementedException("TODO: Implement WOR_World.UnknownStruct");
			}
		}
	}
}
