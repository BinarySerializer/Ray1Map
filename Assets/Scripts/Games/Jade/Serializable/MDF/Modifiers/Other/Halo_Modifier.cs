using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class Halo_Modifier : MDF_Modifier {
		public uint Version { get; set; }
		public float CenterSize { get; set; }
		public float Float_UnderV3_0 { get; set; }
		public float HaloSize { get; set; }
		public float Float_UnderV3_1 { get; set; }
		public float Distance { get; set; }
		public Jade_Vector HaloLocalPos { get; set; }
		public float MinTransparency { get; set; }
		public float MaxTransparency { get; set; }
		public float Angle { get; set; }
		public Jade_Reference<GEO_Object> Material { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			CenterSize = s.Serialize<float>(CenterSize, name: nameof(CenterSize));
			if (Version < 3) Float_UnderV3_0 = s.Serialize<float>(Float_UnderV3_0, name: nameof(Float_UnderV3_0));
			HaloSize = s.Serialize<float>(HaloSize, name: nameof(HaloSize));
			if (Version < 3) Float_UnderV3_1 = s.Serialize<float>(Float_UnderV3_1, name: nameof(Float_UnderV3_1));
			Distance = s.Serialize<float>(Distance, name: nameof(Distance));
			HaloLocalPos = s.SerializeObject<Jade_Vector>(HaloLocalPos, name: nameof(HaloLocalPos));
			MinTransparency = s.Serialize<float>(MinTransparency, name: nameof(MinTransparency));
			MaxTransparency = s.Serialize<float>(MaxTransparency, name: nameof(MaxTransparency));
			if (Version < 3) Angle = s.Serialize<float>(Angle, name: nameof(Angle));
			Material = s.SerializeObject<Jade_Reference<GEO_Object>>(Material, name: nameof(Material))?.Resolve();
		}
	}
}
