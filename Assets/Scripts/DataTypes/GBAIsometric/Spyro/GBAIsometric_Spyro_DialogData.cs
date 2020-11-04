namespace R1Engine
{
    public class GBAIsometric_Spyro_DialogData : R1Serializable
    {
        public ushort Ushort_00 { get; set; } // Always 16384
        public ushort PortraitIndex { get; set; } // Value & 0xFFF

        // Remaining data varies in size depending on the text. There seems to be a system in place for dialog trees, with branching paths based on dialog choices.

        public override void SerializeImpl(SerializerObject s)
        {
            Ushort_00 = s.Serialize<ushort>(Ushort_00, name: nameof(Ushort_00));
            PortraitIndex = s.Serialize<ushort>(PortraitIndex, name: nameof(PortraitIndex));
        }
    }
}