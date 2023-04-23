using BinarySerializer;

namespace Ray1Map.Jade {
    public class SND_TVP_Bank : Jade_File {
        public uint Version { get; set; }
        public string Name { get; set; }
        public uint Count { get; set; }
        public Entry[] Entries { get; set; }

        protected override void SerializeFile(SerializerObject s) {
            Version = s.Serialize<uint>(Version, name: nameof(Version));
            if (Version == 1) {
                Name = s.SerializeString(Name, length: 255, name: nameof(Name));
                Count = s.Serialize<uint>(Count, name: nameof(Count));
                Entries = s.SerializeObjectArray<Entry>(Entries, Count, name: nameof(Entries));
            }
        }

		public class Entry : BinarySerializable {
            public uint ToRead { get; set; }
            public string Name { get; set; }
            public uint UInt_00 { get; set; }
			public uint UInt_04 { get; set; }
			public uint UInt_08 { get; set; }
			public uint UInt_0C { get; set; }
			public uint UInt_10 { get; set; }
			public uint UInt_14 { get; set; }
			public uint UInt_18 { get; set; }
			public uint UInt_1C { get; set; }
			public uint UInt_20 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
                ToRead = s.Serialize<uint>(ToRead, name: nameof(ToRead));
                if (ToRead != 0) {
                    Name = s.SerializeString(Name, length: 255, name: nameof(Name));
                    UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
                    UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
                    UInt_08 = s.Serialize<uint>(UInt_08, name: nameof(UInt_08));
                    UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
                    UInt_10 = s.Serialize<uint>(UInt_10, name: nameof(UInt_10));
                    UInt_14 = s.Serialize<uint>(UInt_14, name: nameof(UInt_14));
                    UInt_18 = s.Serialize<uint>(UInt_18, name: nameof(UInt_18));
                    UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
                    UInt_20 = s.Serialize<uint>(UInt_20, name: nameof(UInt_20));
                }
            }
		}
	}
}