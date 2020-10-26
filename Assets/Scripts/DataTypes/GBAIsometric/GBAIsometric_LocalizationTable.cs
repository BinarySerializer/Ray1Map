namespace R1Engine
{
    public class GBAIsometric_LocalizationTable : R1Serializable
    {
        public Pointer<Array<ushort>>[] Offsets { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Offsets = s.SerializePointerArray<Array<ushort>>(Offsets, 6, resolve: true, onPreSerialize: x => x.Length = 690, name: nameof(Offsets));
        }
    }
}