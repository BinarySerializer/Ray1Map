using System.Linq;

namespace R1Engine
{
    public class GBC_ROM : GBC_ROMBase
    {
        public ushort ReferencesCount { get; set; }
        public byte Byte_02 { get; set; } // Engine version?
        public Reference[] References { get; set; }

        public GBC_LevelList LevelList { get; set; }
        //public GBC_SoundBank SoundBank { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var memoryBank = 15;

            if (s.GameSettings.GameModeSelection == GameModeSelection.MowgliGBC)
                memoryBank = 31;

            // Serialize data
            s.DoAt(GBC_Pointer.GetPointer(Offset, GBC_Pointer.MemoryBankBaseAddress, memoryBank), () =>
            {
                ReferencesCount = s.Serialize<ushort>(ReferencesCount, name: nameof(ReferencesCount));
                Byte_02 = s.Serialize<byte>(Byte_02, name: nameof(Byte_02));
                References = s.SerializeObjectArray<Reference>(References, ReferencesCount, name: nameof(References));
            });

            LevelList = s.DoAt(References.First(x => x.BlockHeader.Type == GBC_BlockType.LevelList).Pointer.GetPointer(), () => s.SerializeObject<GBC_LevelList>(LevelList, name: nameof(LevelList)));
            //SoundBank = s.DoAt(References.First(x => x.BlockHeader.Type == GBC_BlockType.SoundBank).Pointer.GetPointer(), () => s.SerializeObject<GBC_SoundBank>(SoundBank, name: nameof(SoundBank)));

        }

        public class Reference : R1Serializable
        {
            public GBC_BlockHeader BlockHeader { get; set; }
            public GBC_Pointer Pointer { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                BlockHeader = s.SerializeObject<GBC_BlockHeader>(BlockHeader, name: nameof(BlockHeader));
                Pointer = s.SerializeObject<GBC_Pointer>(Pointer, name: nameof(Pointer));
            }
        }
    }
}