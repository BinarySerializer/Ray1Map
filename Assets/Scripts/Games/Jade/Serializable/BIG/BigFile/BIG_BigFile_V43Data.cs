using BinarySerializer;

namespace Ray1Map.Jade {
	public class BIG_BigFile_V43Data : BinarySerializable {
		public uint UInt_00 { get; set; }
		public Jade_Key VersionTag { get; set; }
		public Jade_Key EditorTexture { get; set; }
		public Jade_Key DefaultTexture { get; set; }
		public Jade_Key DebugFontTexture { get; set; }
		public Jade_Key DebugFontDescriptor { get; set; }
		public Jade_Key GMat { get; set; }
		public Jade_Key SystemAIFunction { get; set; }
		public Jade_Key Icon { get; set; }
		public Jade_Key LoadingTextureHigh { get; set; }
		public Jade_Key LoadingTextureNormal { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			VersionTag = s.SerializeObject<Jade_Key>(VersionTag, name: nameof(VersionTag));
			EditorTexture = s.SerializeObject<Jade_Key>(EditorTexture, name: nameof(EditorTexture));
			DefaultTexture = s.SerializeObject<Jade_Key>(DefaultTexture, name: nameof(DefaultTexture));
			DebugFontTexture = s.SerializeObject<Jade_Key>(DebugFontTexture, name: nameof(DebugFontTexture));
			DebugFontDescriptor = s.SerializeObject<Jade_Key>(DebugFontDescriptor, name: nameof(DebugFontDescriptor));
			GMat = s.SerializeObject<Jade_Key>(GMat, name: nameof(GMat));
			SystemAIFunction = s.SerializeObject<Jade_Key>(SystemAIFunction, name: nameof(SystemAIFunction));
			Icon = s.SerializeObject<Jade_Key>(Icon, name: nameof(Icon));
			LoadingTextureHigh = s.SerializeObject<Jade_Key>(LoadingTextureHigh, name: nameof(LoadingTextureHigh));
			LoadingTextureNormal = s.SerializeObject<Jade_Key>(LoadingTextureNormal, name: nameof(LoadingTextureNormal));
		}
	}
}
