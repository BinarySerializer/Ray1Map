using BinarySerializer;

namespace Ray1Map.Gameloft
{
    public abstract class Gameloft_Resource : BinarySerializable
    {
        // Set in onPreSerialize
        public uint ResourceSize { get; set; }
    }
}