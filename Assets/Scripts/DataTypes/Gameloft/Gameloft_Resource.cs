namespace R1Engine
{
    public abstract class Gameloft_Resource : R1Serializable
    {
        // Set in onPreSerialize
        public uint ResourceSize { get; set; }
    }
}