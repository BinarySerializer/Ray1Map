namespace R1Engine
{
    public class R1_PC_WorldMapLevelMapEntry : R1Serializable
    {
        public sbyte World { get; set; }
        public sbyte Level { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            World = s.Serialize<sbyte>(World, name: nameof(World));
            Level = s.Serialize<sbyte>(Level, name: nameof(Level));
        }
    }
}