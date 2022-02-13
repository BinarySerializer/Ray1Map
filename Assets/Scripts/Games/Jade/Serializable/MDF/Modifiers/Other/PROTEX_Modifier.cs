using BinarySerializer;

namespace Ray1Map.Jade {
	public class PROTEX_Modifier : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint Version { get; set; }
		public uint NumberOfHLines { get; set; }
		public byte[] AllDotNumbers { get; set; }
		public uint NumberOfDots { get; set; }
		public byte[] AllDot { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			Version = s.Serialize<uint>(Version, name: nameof(Version));
			if (Version != 0) {
				NumberOfHLines = s.Serialize<uint>(NumberOfHLines, name: nameof(NumberOfHLines));
				AllDotNumbers = s.SerializeArray<byte>(AllDotNumbers, NumberOfHLines, name: nameof(AllDotNumbers));
				NumberOfDots = s.Serialize<uint>(NumberOfDots, name: nameof(NumberOfDots));
				AllDot = s.SerializeArray<byte>(AllDot, NumberOfDots, name: nameof(AllDot));
			}

		}
	}
}
