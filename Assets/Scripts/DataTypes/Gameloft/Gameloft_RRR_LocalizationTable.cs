namespace R1Engine
{
	public class Gameloft_RRR_LocalizationTable : Gameloft_Resource {
		public LanguageTable[] LanguageTables { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			s.DoEndian(R1Engine.Serialize.BinaryFile.Endian.Big, () => {
				LanguageTables = s.SerializeArraySize<LanguageTable, ushort>(LanguageTables, name: nameof(LanguageTables));
				LanguageTables = s.SerializeObjectArray<LanguageTable>(LanguageTables, LanguageTables.Length, name: nameof(LanguageTables));
			});
		}

		public class LanguageTable : R1Serializable {
			public uint Length { get; set; }
			public string[] Strings { get; set; }
			
			public override void SerializeImpl(SerializerObject s) {
				Length = s.Serialize<uint>(Length, name: nameof(Length));
				Strings = s.SerializeArraySize<string, ushort>(Strings, name: nameof(Strings));
				for (int i = 0; i < Strings.Length; i++) {
					ushort len = s.Serialize<ushort>((ushort)(Strings[i]?.Length ?? 0), name: $"{nameof(Strings)}[{i}].Length");
					Strings[i] = s.SerializeString(Strings[i], length: len, name: $"{nameof(Strings)}[{i}]");
				}
			}
		}
	}
}