using BinarySerializer;

namespace R1Engine
{
    public class PS1Klonoa_ArchiveFile_PathBlock : BinarySerializable
    {
        // TODO: Parse
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, 28, name: nameof(Data));
        }
    }
}