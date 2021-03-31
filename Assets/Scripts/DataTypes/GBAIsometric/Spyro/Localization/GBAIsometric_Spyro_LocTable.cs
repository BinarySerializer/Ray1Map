using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro_LocTable : BinarySerializable
    {
        public ushort ID { get; set; }
        public ushort StartIndex { get; set; }
        public ushort NumEntries { get; set; }
        public ushort Padding { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ID = s.Serialize<ushort>(ID, name: nameof(ID));
            StartIndex = s.Serialize<ushort>(StartIndex, name: nameof(StartIndex));
            NumEntries = s.Serialize<ushort>(NumEntries, name: nameof(NumEntries));
            Padding = s.Serialize<ushort>(Padding, nameof(Padding));
        }
    }
}