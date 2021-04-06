using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_ActionData : BinarySerializable {
		public Jade_Reference<EVE_ListTracks> ListTracks { get; set; }
		public Jade_Reference<ANI_Shape> Shape { get; set; }
		public Jade_Reference<GRP_Grp> GRP { get; set; }
		public Jade_Reference<ACT_ActionKit> ActionKit { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// These are resolved later after loading the world
			ListTracks = s.SerializeObject<Jade_Reference<EVE_ListTracks>>(ListTracks, name: nameof(ListTracks));
			Shape = s.SerializeObject<Jade_Reference<ANI_Shape>>(Shape, name: nameof(Shape));
			GRP = s.SerializeObject<Jade_Reference<GRP_Grp>>(GRP, name: nameof(GRP));

			// This is resolved
			ActionKit = s.SerializeObject<Jade_Reference<ACT_ActionKit>>(ActionKit, name: nameof(ActionKit))?.Resolve();
		}
	}
}
