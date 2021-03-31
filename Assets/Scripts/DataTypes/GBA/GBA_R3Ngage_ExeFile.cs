using BinarySerializer;

namespace R1Engine
{
    public class GBA_R3Ngage_ExeFile : BinarySerializable
    {
        public GBA_LocLanguageTable Localization { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var pointerTable = PointerTables.GBA_PointerTable(s.Context, Offset.File);

            s.DoAt(pointerTable[GBA_Pointer.Localization], () => Localization = s.SerializeObject<GBA_LocLanguageTable>(Localization, name: nameof(Localization)));
        }
    }
}