using BinarySerializer;

namespace R1Engine.Jade
{
    public class Jade_DummyFile : Jade_File
    {
        public override void SerializeImpl(SerializerObject s)
        {
            s.Goto(Offset + FileSize);
        }
    }
}