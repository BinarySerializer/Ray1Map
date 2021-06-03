using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierSpring : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Version { get; set; }
		public uint Flags { get; set; }
		public float SpringConstant { get; set; }
		public float DampingConstant { get; set; }
		public OBJ_BV_Box Box { get; set; }
		public Jade_Vector Gravity { get; set; }
		public float ForceWind { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version >= 3) Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			if (Version >= 1) {
				SpringConstant = s.Serialize<float>(SpringConstant, name: nameof(SpringConstant));
				DampingConstant = s.Serialize<float>(DampingConstant, name: nameof(DampingConstant));
				Box = s.SerializeObject<OBJ_BV_Box>(Box, name: nameof(Box));
				Gravity = s.SerializeObject<Jade_Vector>(Gravity, name: nameof(Gravity));
			}
			if (Version >= 2) ForceWind = s.Serialize<float>(ForceWind, name: nameof(ForceWind));
		}
	}
}
