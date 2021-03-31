using BinarySerializer;

namespace R1Engine
{
    public abstract class Gameloft_Resource : BinarySerializable
    {
        // Set in onPreSerialize
        public uint ResourceSize { get; set; }
    }
}