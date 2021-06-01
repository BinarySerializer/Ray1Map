using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_ul_ModifierOnduleTonCorps_Load
	public class GEO_ModifierOnduleTonCorps : MDF_Modifier {
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public float Angle { get; set; }
		public float Amplitude { get; set; }
		public float Factor { get; set; }
		public float Delta { get; set; }
		public uint MatID { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))
				Version = s.Serialize<uint>(Version, name: nameof(Version));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Angle = s.Serialize<float>(Angle, name: nameof(Angle));
			Amplitude = s.Serialize<float>(Amplitude, name: nameof(Amplitude));
			Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			Delta = s.Serialize<float>(Delta, name: nameof(Delta));
			if (!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))
				MatID = s.Serialize<uint>(MatID, name: nameof(MatID));
		}
	}
}
