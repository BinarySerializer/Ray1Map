using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	// Found in GEO_ul_ModifierOnduleTonCorps_Load
	public class GEO_ModifierOnduleTonCorps : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Flags { get; set; }
		public float Angle { get; set; }
		public float Amplitude { get; set; }
		public float Factor { get; set; }
		public float Delta { get; set; }
		public uint UInt_Editor_14 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			Angle = s.Serialize<float>(Angle, name: nameof(Angle));
			Amplitude = s.Serialize<float>(Amplitude, name: nameof(Amplitude));
			Factor = s.Serialize<float>(Factor, name: nameof(Factor));
			Delta = s.Serialize<float>(Delta, name: nameof(Delta));
			if (!Loader.IsBinaryData) UInt_Editor_14 = s.Serialize<uint>(UInt_Editor_14, name: nameof(UInt_Editor_14));
		}
	}
}
