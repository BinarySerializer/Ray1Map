using BinarySerializer;

namespace R1Engine
{
    /// <summary>
    /// A file where the data gets serialized as raw bytes
    /// </summary>
    public class PS1Klonoa_File_RawData : PS1Klonoa_BaseFile
    {
        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, Pre_FileSize, name: nameof(Data));
        }
    }
}