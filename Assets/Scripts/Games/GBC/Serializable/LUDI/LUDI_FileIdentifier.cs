using BinarySerializer;

namespace Ray1Map.GBC
{
    public class LUDI_FileIdentifier : BinarySerializable {
        public ushort Unknown { get; set; }
        public ushort FileID { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Unknown = s.Serialize<ushort>(Unknown, name: nameof(Unknown));
            FileID = s.Serialize<ushort>(FileID, name: nameof(FileID));
        }

        public bool Match(LUDI_FileIdentifier other) {
            if(other == null) return false;
            return (other.Unknown == Unknown && other.FileID == FileID);
        }
    }
}