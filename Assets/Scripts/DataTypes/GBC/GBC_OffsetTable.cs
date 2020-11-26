using System.Collections.Generic;

namespace R1Engine
{
    public class GBC_OffsetTable : R1Serializable {
        public uint OffsetsCount { get; set; }
        public GBC_Offset[] Offsets { get; set; }


        public static List<GBC_OffsetTable> OffsetTables { get; } = new List<GBC_OffsetTable>();
        public bool[] UsedOffsets { get; set; }

        public override void SerializeImpl(SerializerObject s) 
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
		        OffsetsCount = s.Serialize<byte>((byte)OffsetsCount, name: nameof(OffsetsCount));
            else
		        OffsetsCount = s.Serialize<uint>(OffsetsCount, name: nameof(OffsetsCount));
            Offsets = s.SerializeObjectArray<GBC_Offset>(Offsets, OffsetsCount, name: nameof(Offsets));

            // For export
            UsedOffsets = new bool[OffsetsCount];
            OffsetTables.Add(this);
        }

        public Pointer GetPointer(int index) {
            UsedOffsets[index] = true;

            if (Context.Settings.EngineVersion == EngineVersion.GBC_R1_Palm ||
                Context.Settings.EngineVersion == EngineVersion.GBC_R1_PocketPC) {
                var offTable = Context.GetStoredObject<LUDI_GlobalOffsetTable>(GBC_BaseManager.GlobalOffsetTableKey);
                return offTable?.Resolve(Offsets[index]);
            } else {
                var ptr = Offsets[index];
                return ptr.GBC_Pointer.GetPointer();
            }
        }
    }
}