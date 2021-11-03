using BinarySerializer;

namespace Ray1Map.GBC
{
    public class LUDI_Header : LUDI_AppInfoBlock {
        public uint LUDI { get; set; }
        public LUDI_FileIdentifier FileID { get; set; }
        public FileType Type { get; set; }

		public override void SerializeBlock(SerializerObject s) {
		    LUDI = s.Serialize<uint>(LUDI, name: nameof(LUDI));
            FileID = s.SerializeObject<LUDI_FileIdentifier>(FileID, name: nameof(FileID));
            Type = s.Serialize<FileType>(Type, name: nameof(Type));
        }

        public enum FileType : uint {
            DataFile = 0,
            SpecialFile = 1
        }
    }
}