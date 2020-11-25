namespace R1Engine
{
    public class LUDI_Header : LUDI_Block {
        public uint LUDI { get; set; }
        public ushort Unknown { get; set; }
        public ushort FileIndex { get; set; }
        public FileType Type { get; set; }

		public override void SerializeBlock(SerializerObject s) {
		    LUDI = s.Serialize<uint>(LUDI, name: nameof(LUDI));
            Unknown = s.Serialize<ushort>(Unknown, name: nameof(Unknown));
            FileIndex = s.Serialize<ushort>(FileIndex, name: nameof(FileIndex));
            Type = s.Serialize<FileType>(Type, name: nameof(Type));
        }

        public enum FileType : uint {
            DataFile = 0,
            SpecialFile = 1
        }
    }
}