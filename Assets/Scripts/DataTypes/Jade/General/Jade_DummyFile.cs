using BinarySerializer;

namespace R1Engine.Jade
{
    public class Jade_DummyFile : Jade_File
    {
        protected override void SerializeFile(SerializerObject s)
        {
            s.Goto(Offset + FileSize);
        }
    }
}