using BinarySerializer;

namespace R1Engine
{
	public class Gameloft_RK_LocalizationTable : Gameloft_Resource {
		public uint StringsCount { get; set; }
		public uint[] StringOffsets { get; set; }
		public string[] Strings { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			StringsCount = s.Serialize<uint>(StringsCount, name: nameof(StringsCount));
			StringOffsets = s.SerializeArray<uint>(StringOffsets, StringsCount, name: nameof(StringOffsets));
			Pointer stringBase = s.CurrentPointer;
			if(Strings == null) Strings = new string[StringsCount];
			for (int i = 0; i < Strings.Length; i++) {
				Strings[i] = s.SerializeString(Strings[i], StringOffsets[i] - (i > 0 ? StringOffsets[i-1] : 0), encoding: System.Text.Encoding.GetEncoding(1252), name: $"{nameof(Strings)}[{i}]");
			}
		}
	}
}