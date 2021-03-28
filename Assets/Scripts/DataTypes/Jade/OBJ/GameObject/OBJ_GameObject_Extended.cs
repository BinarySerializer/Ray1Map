using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Extended : R1Serializable {
		public OBJ_GameObject_IdentityFlags FlagsIdentity { get; set; } // Set in OnPreSerialize

		// GRP
		public Jade_Reference<GRP_Grp> GRP { get; set; }
		public uint HasModifiers { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GRP = s.SerializeObject<Jade_Reference<GRP_Grp>>(GRP, name: nameof(GRP));
			if (FlagsIdentity.HasFlag(OBJ_GameObject_IdentityFlags.Flag23)) GRP?.Resolve();
			HasModifiers = s.Serialize<uint>(HasModifiers, name: nameof(HasModifiers));
			throw new NotImplementedException($"TODO: Implement {GetType()}");
		}
	}
}
