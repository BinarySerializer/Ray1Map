using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GRA_GrassField_CreateFromBuffer
	public class GRA_GrassField : GRO_GraphicRenderObject {
		public uint Flags { get; set; }
		public uint TileSize { get; set; }
		public uint TileCountX { get; set; }
		public uint TileCountY { get; set; }
		public float FieldHeight { get; set; }

		public Jade_TextureReference Texture { get; set; }
		public uint AlphaTestRef { get; set; }
		public uint NumVarietyU { get; set; }
		public uint NumVarietyV { get; set; }
		public float ShearFactor { get; set; }

		public float QuickWindFrequency { get; set; }
		public float QuickWindStrength { get; set; }
		public float QW_Float0 { get; set; }
		public float QW_Float1 { get; set; }
		public float QW_Float2 { get; set; }
		public float QW_Float3 { get; set; }
		public float QW_Float4 { get; set; }
		public float QW_Float5 { get; set; }
		public uint QW_UInt6 { get; set; }
		public float QW_Float7 { get; set; }
		public float QW_Float8 { get; set; }
		public float QW_Float9 { get; set; }

		public GRA_GrassType[] GrassTypes { get; set; }
		public GRA_PatternOptions PatternOptions { get; set; }
		public GRA_GrassTree Tree { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			TileSize = s.Serialize<uint>(TileSize, name: nameof(TileSize));
			TileCountX = s.Serialize<uint>(TileCountX, name: nameof(TileCountX));
			TileCountY = s.Serialize<uint>(TileCountY, name: nameof(TileCountY));
			FieldHeight = s.Serialize<float>(FieldHeight, name: nameof(FieldHeight));

			if (ObjectVersion >= 6) {
				Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture))?.Resolve();
				AlphaTestRef = s.Serialize<uint>(AlphaTestRef, name: nameof(AlphaTestRef));
				NumVarietyU = s.Serialize<uint>(NumVarietyU, name: nameof(NumVarietyU));
				NumVarietyV = s.Serialize<uint>(NumVarietyV, name: nameof(NumVarietyV));
				if (ObjectVersion >= 7) ShearFactor = s.Serialize<float>(ShearFactor, name: nameof(ShearFactor));
			}
			if (ObjectVersion >= 4) {
				QuickWindFrequency = s.Serialize<float>(QuickWindFrequency, name: nameof(QuickWindFrequency));
				QuickWindStrength = s.Serialize<float>(QuickWindStrength, name: nameof(QuickWindStrength));
				if (ObjectVersion < 6) {
					QW_Float0 = s.Serialize<float>(QW_Float0, name: nameof(QW_Float0));
					QW_Float1 = s.Serialize<float>(QW_Float1, name: nameof(QW_Float1));
					QW_Float2 = s.Serialize<float>(QW_Float2, name: nameof(QW_Float2));
					QW_Float3 = s.Serialize<float>(QW_Float3, name: nameof(QW_Float3));
					QW_Float4 = s.Serialize<float>(QW_Float4, name: nameof(QW_Float4));
					QW_Float5 = s.Serialize<float>(QW_Float5, name: nameof(QW_Float5));
					QW_UInt6 = s.Serialize<uint>(QW_UInt6, name: nameof(QW_UInt6));
					QW_Float7 = s.Serialize<float>(QW_Float7, name: nameof(QW_Float7));
					QW_Float8 = s.Serialize<float>(QW_Float8, name: nameof(QW_Float8));
					QW_Float9 = s.Serialize<float>(QW_Float9, name: nameof(QW_Float9));
				}
			}
			GrassTypes = s.SerializeObjectArray<GRA_GrassType>(GrassTypes, 4, onPreSerialize: gt => gt.Pre_ObjectVersion = ObjectVersion, name: nameof(GrassTypes));
			PatternOptions = s.SerializeObject<GRA_PatternOptions>(PatternOptions, onPreSerialize: po => po.Pre_ObjectVersion = ObjectVersion, name: nameof(PatternOptions));
			Tree = s.SerializeObject<GRA_GrassTree>(Tree, onPreSerialize: gt => gt.Pre_ObjectVersion = ObjectVersion, name: nameof(Tree));
		}
	}
}
