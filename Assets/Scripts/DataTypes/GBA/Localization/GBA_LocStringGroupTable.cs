namespace R1Engine
{
    public class GBA_LocStringGroupTable : R1Serializable
    {
        public Pointer[] Pointers { get; set; }

        public GBA_LocStringTable[] LocStrings { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointers = s.SerializePointerArray(Pointers, 12, name: nameof(Pointers));

            if (LocStrings == null)
                LocStrings = new GBA_LocStringTable[Pointers.Length];

            for (int i = 0; i < LocStrings.Length; i++)
                LocStrings[i] = s.DoAt(Pointers[i], () => s.SerializeObject<GBA_LocStringTable>(LocStrings[i], name: $"{nameof(LocStrings)}[{i}]"));
        }
    }
}