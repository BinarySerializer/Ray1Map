using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_DesignStruct : BinarySerializable {
		public uint UInt_00 { get; set; }
		public int Flags { get; set; }
		public int I1 { get; set; }
		public int I2 { get; set; }
		public float F1 { get; set; }
		public float F2 { get; set; }
		public Jade_Vector Vec1 { get; set; }
		public Jade_Vector Vec2 { get; set; }
		public uint Perso1 { get; set; }
		public uint Perso2 { get; set; }
		public uint Net1 { get; set; }
		public uint Net2 { get; set; }
		public TEXT_Eval Text1 { get; set; }
		public TEXT_Eval Text2 { get; set; }
		public uint DesignFlags { get; set; }
		public uint DesignFlagsInit { get; set; }
		public int I3 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Flags = s.Serialize<int>(Flags, name: nameof(Flags));
			I1 = s.Serialize<int>(I1, name: nameof(I1));
			I2 = s.Serialize<int>(I2, name: nameof(I2));
			F1 = s.Serialize<float>(F1, name: nameof(F1));
			F2 = s.Serialize<float>(F2, name: nameof(F2));
			Vec1 = s.SerializeObject<Jade_Vector>(Vec1, name: nameof(Vec1));
			Vec2 = s.SerializeObject<Jade_Vector>(Vec2, name: nameof(Vec2));
			Perso1 = s.Serialize<uint>(Perso1, name: nameof(Perso1));
			Perso2 = s.Serialize<uint>(Perso2, name: nameof(Perso2));
			Net1 = s.Serialize<uint>(Net1, name: nameof(Net1));
			Net2 = s.Serialize<uint>(Net2, name: nameof(Net2));
			Text1 = s.SerializeObject<TEXT_Eval>(Text1, name: nameof(Text1));
			Text2 = s.SerializeObject<TEXT_Eval>(Text2, name: nameof(Text2));
			DesignFlags = s.Serialize<uint>(DesignFlags, name: nameof(DesignFlags));
			DesignFlagsInit = s.Serialize<uint>(DesignFlagsInit, name: nameof(DesignFlagsInit));
			I3 = s.Serialize<int>(I3, name: nameof(I3));
		}

		public class TEXT_Eval : BinarySerializable {
			public Jade_Key FileKey { get; set; }
			public int ID { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				FileKey = s.SerializeObject<Jade_Key>(FileKey, name: nameof(FileKey));
				ID = s.Serialize<int>(ID, name: nameof(ID));
			}
		}
	}
}
