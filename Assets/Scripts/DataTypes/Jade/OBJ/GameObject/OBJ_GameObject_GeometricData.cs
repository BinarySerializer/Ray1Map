using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class OBJ_GameObject_GeometricData : R1Serializable {
		public Jade_Reference<GEO_Object> GeometricObject0 { get; set; }
		public Jade_Reference<GEO_Object> GeometricObject1 { get; set; }
		public int Int_7A { get; set; }
		public uint UInt_7E { get; set; } // Seem to be flags of some sort
		public uint Code { get; set; }
		public Jade_Reference<OBJ_GameObjectRLI> RLI { get; set; }
		public uint[] RLI_UInts { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GeometricObject0 = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject0, name: nameof(GeometricObject0))?.Resolve();
			GeometricObject1 = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject1, name: nameof(GeometricObject1))?.Resolve();
			Int_7A = s.Serialize<int>(Int_7A, name: nameof(Int_7A));
			UInt_7E = s.Serialize<uint>(UInt_7E, name: nameof(UInt_7E));
			Code = s.Serialize<uint>(Code, name: nameof(Code));
			if (Code == (uint)Jade_Code.RLI) {
				RLI = s.SerializeObject<Jade_Reference<OBJ_GameObjectRLI>>(RLI, name: nameof(RLI))?.Resolve();
			} else {
				RLI_UInts = s.SerializeArray<uint>(RLI_UInts, Code, name: nameof(RLI_UInts));
			}
		}
	}
}
