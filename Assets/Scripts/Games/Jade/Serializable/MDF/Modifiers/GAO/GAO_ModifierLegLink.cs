using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_ul_ModifierOnduleTonCorps_Load
	public class GAO_ModifierLegLink : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObjectA { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObjectC { get; set; }
		public float AB { get; set; }
		public float BC { get; set; }
		public Jade_Reference<OBJ_GameObject> GameObjectOrient { get; set; }
		public uint Flags { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			GameObjectA = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObjectA, name: nameof(GameObjectA));
			GameObjectC = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObjectC, name: nameof(GameObjectC));
			AB = s.Serialize<float>(AB, name: nameof(AB));
			BC = s.Serialize<float>(BC, name: nameof(BC));
			GameObjectOrient = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObjectOrient, name: nameof(GameObjectOrient));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			if ((Flags & 0x10) == 0) {
				GameObjectA?.Resolve();
				GameObjectC?.Resolve();
				GameObjectOrient?.Resolve();
			}
		}
	}
}
