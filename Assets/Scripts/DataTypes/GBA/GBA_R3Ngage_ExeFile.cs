namespace R1Engine
{
    public class GBA_R3Ngage_ExeFile : R1Serializable
    {
        public GBA_LocLanguageTable Localization { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var pointerTable = PointerTables.GBA_PointerTable(s.Context, Offset.file);

            s.DoAt(pointerTable[GBA_Pointer.Localization], () => Localization = s.SerializeObject<GBA_LocLanguageTable>(Localization, name: nameof(Localization)));
        }
    }
}