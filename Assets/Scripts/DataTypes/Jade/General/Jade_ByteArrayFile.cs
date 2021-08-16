using BinarySerializer;

namespace R1Engine.Jade
{
    public class Jade_ByteArrayFile : Jade_File
    {
        public byte[] Bytes { get; set; }

        protected override void SerializeFile(SerializerObject s)
        {
			Bytes = s.SerializeArray<byte>(Bytes, FileSize, name: nameof(Bytes));
		}
    }
}