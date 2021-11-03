using BinarySerializer;

namespace Ray1Map.Jade
{
    public class Jade_DummyFile : Jade_File
    {
        protected override void SerializeFile(SerializerObject s)
        {
            s.Goto(Offset + FileSize);
        }
    }
}