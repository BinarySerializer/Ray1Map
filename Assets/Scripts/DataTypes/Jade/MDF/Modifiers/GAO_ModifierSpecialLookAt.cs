using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierSpecialLookAt : MDF_Modifier {
		public uint Version { get; set; }
		public byte Type { get; set; }
		public byte Dummy { get; set; }
		public byte Byte_06 { get; set; }
		public byte Byte_07 { get; set; }
		public float Z { get; set; }
		public int GaoRank { get; set; } = -1;
		public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
		public float Value1 { get; set; } = 1000f;
		public float Value2 { get; set; } = 1000f;
		public uint AnimationCanal { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			Type = s.Serialize<byte>(Type, name: nameof(Type));
			Dummy = s.Serialize<byte>(Dummy, name: nameof(Dummy));
			Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
			Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
			Z = s.Serialize<float>(Z, name: nameof(Z));
			if(Version == 28 || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))
				GaoRank = s.Serialize<int>(GaoRank, name: nameof(GaoRank));
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
			if(GaoRank == -1) GameObject?.Resolve();
			if (Version != 4 || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
				Value1 = s.Serialize<float>(Value1, name: nameof(Value1));
				Value2 = s.Serialize<float>(Value2, name: nameof(Value2));
			}
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Version >= 29) {
				AnimationCanal = s.Serialize<uint>(AnimationCanal, name: nameof(AnimationCanal));
			}
		}
	}
}
