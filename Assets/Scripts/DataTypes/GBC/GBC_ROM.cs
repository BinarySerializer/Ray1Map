using System.Linq;

namespace R1Engine
{
    public class GBC_ROM : GBC_ROMBase
    {
        public ushort ReferencesCount { get; set; }
        public byte Byte_02 { get; set; } // Engine version?
        public Reference[] References { get; set; }

        public GBC_LevelList LevelList { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var memoryBank = 15;

            if (s.GameSettings.GameModeSelection == GameModeSelection.MowgliGBC)
                memoryBank = 31;

            // Serialize data
            s.DoAt(GBC_Pointer.GetPointer(Offset, 0x4000, memoryBank), () =>
            {
                ReferencesCount = s.Serialize<ushort>(ReferencesCount, name: nameof(ReferencesCount));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                References = s.SerializeObjectArray<Reference>(References, ReferencesCount, name: nameof(References));
            });

            // TODO: Clean up
            LevelList = s.DoAt(References.First(x => x.Bytes_00[2] == 0x17).Pointer.GetPointer(), () => s.SerializeObject<GBC_LevelList>(LevelList, name: nameof(LevelList)));
        }

        public class Reference : R1Serializable
        {
            public byte[] Bytes_00 { get; set; }
            public GBC_Pointer Pointer { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Bytes_00 = s.SerializeArray<byte>(Bytes_00, 5, name: nameof(Bytes_00));
                Pointer = s.SerializeObject<GBC_Pointer>(Pointer, name: nameof(Pointer));
            }
        }
    }
}