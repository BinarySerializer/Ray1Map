namespace R1Engine
{
    public class GBA_LocLanguageTable : R1Serializable
    {
        public Pointer[] Pointers { get; set; }
        
        public GBA_LocStringGroupTable[] StringGroups { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Pointers = s.SerializePointerArray(Pointers, 10, name: nameof(Pointers));

            if (StringGroups == null)
                StringGroups = new GBA_LocStringGroupTable[Pointers.Length];

            for (int i = 0; i < StringGroups.Length; i++)
                StringGroups[i] = s.DoAt(Pointers[i], () => s.SerializeObject<GBA_LocStringGroupTable>(StringGroups[i], name:  $"{nameof(StringGroups)}[{i}]"));
        }
    }
}