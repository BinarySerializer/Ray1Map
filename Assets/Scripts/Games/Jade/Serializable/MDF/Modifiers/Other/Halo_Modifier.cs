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

		// CPP
		public float CentralAttenuationColor { get; set; }
		public float CentralAttenuationSize { get; set; }
		public float DistanceAttenuation { get; set; }
		public float AttenuationSpeed { get; set; }
		public uint SubdivisionsCountU { get; set; }
		public uint SubdivisionsCountV { get; set; }
		public uint HalosCount { get; set; }
		public Halo[] Halos { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				HaloLocalPos = s.SerializeObject<Jade_Vector>(HaloLocalPos, name: nameof(HaloLocalPos));
				Material = s.SerializeObject<Jade_Reference<GEO_Object>>(Material, name: nameof(Material))?.Resolve();
				CentralAttenuationColor = s.Serialize<float>(CentralAttenuationColor, name: nameof(CentralAttenuationColor));
				CentralAttenuationSize = s.Serialize<float>(CentralAttenuationSize, name: nameof(CentralAttenuationSize));
				if(Version >= 6) DistanceAttenuation = s.Serialize<float>(DistanceAttenuation, name: nameof(DistanceAttenuation));
				AttenuationSpeed = s.Serialize<float>(AttenuationSpeed, name: nameof(AttenuationSpeed));
				SubdivisionsCountU = s.Serialize<uint>(SubdivisionsCountU, name: nameof(SubdivisionsCountU));
				SubdivisionsCountV = s.Serialize<uint>(SubdivisionsCountV, name: nameof(SubdivisionsCountV));
				HalosCount = s.Serialize<uint>(HalosCount, name: nameof(HalosCount));
				Halos = s.SerializeObjectArray<Halo>(Halos, HalosCount, name: nameof(Halos));
			} else {
				Version = s.Serialize<uint>(Version, name: nameof(Version));
				CenterSize = s.Serialize<float>(CenterSize, name: nameof(CenterSize));
				if (Version < 3) Float_UnderV3_0 = s.Serialize<float>(Float_UnderV3_0, name: nameof(Float_UnderV3_0));
				HaloSize = s.Serialize<float>(HaloSize, name: nameof(HaloSize));
				if (Version < 3) Float_UnderV3_1 = s.Serialize<float>(Float_UnderV3_1, name: nameof(Float_UnderV3_1));
				Distance = s.Serialize<float>(Distance, name: nameof(Distance));
				HaloLocalPos = s.SerializeObject<Jade_Vector>(HaloLocalPos, name: nameof(HaloLocalPos));
				MinTransparency = s.Serialize<float>(MinTransparency, name: nameof(MinTransparency));
				MaxTransparency = s.Serialize<float>(MaxTransparency, name: nameof(MaxTransparency));
				if (Version > 3) Angle = s.Serialize<float>(Angle, name: nameof(Angle));
				Material = s.SerializeObject<Jade_Reference<GEO_Object>>(Material, name: nameof(Material))?.Resolve();
			}
		}

		public class Halo : BinarySerializable {
			public float HaloSize { get; set; }
			public SerializableColor Color { get; set; }
			public float Distance { get; set; }
			public uint TextureID { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				HaloSize = s.Serialize<float>(HaloSize, name: nameof(HaloSize));
				Color = s.SerializeInto<SerializableColor>(Color, BitwiseColor.RGBA8888, name: nameof(Color));
				Distance = s.Serialize<float>(Distance, name: nameof(Distance));
				TextureID = s.Serialize<uint>(TextureID, name: nameof(TextureID));
			}
		}
	}
}
