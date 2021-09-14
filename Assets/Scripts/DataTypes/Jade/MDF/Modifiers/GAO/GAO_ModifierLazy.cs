using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierLazy : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public uint Version { get; set; }
		public float LazyFactor { get; set; }
		public uint Flags { get; set; }
		public OBJ_BV_Box BoundingVolume { get; set; }
		public float Gravity { get; set; }
		public float Friction { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			LazyFactor = s.Serialize<float>(LazyFactor, name: nameof(LazyFactor));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			if (Version >= 1) BoundingVolume = s.SerializeObject<OBJ_BV_Box>(BoundingVolume, name: nameof(BoundingVolume));
			if (Version >= 2) Gravity = s.Serialize<float>(Gravity, name: nameof(Gravity));
			if (Version >= 3) Friction = s.Serialize<float>(Friction, name: nameof(Friction));
		}
	}
}
