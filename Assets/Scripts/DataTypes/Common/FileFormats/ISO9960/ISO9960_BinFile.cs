using Boo.Lang;
using System.Linq;

namespace R1Engine
{
    // See: https://wiki.osdev.org/ISO_9660
    public class ISO9960_BinFile : R1Serializable {
        public const uint SectorDataSize = 0x800;
        public const uint SectorHeaderSize = 0x18;
        public const uint SectorFooterSize = 0x118;
        public const uint SectorSize = SectorDataSize + SectorHeaderSize + SectorFooterSize;


        public ISO9960_Sector<ISO9960_VolumeDescriptor_Primary> PrimaryVolumeDescriptor { get; set; }
        public ISO9960_Sector<ISO9960_PathTable> PathTable { get; set; }
        public ISO9960_Sector<ISO9960_Directory>[] Directories { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            s.DoAt(LBAToPointer(16), () => {
                PrimaryVolumeDescriptor = s.SerializeObject<ISO9960_Sector<ISO9960_VolumeDescriptor_Primary>>(PrimaryVolumeDescriptor, name: nameof(PrimaryVolumeDescriptor));
            });
            s.DoAt(LBAToPointer(PrimaryVolumeDescriptor.Object.PathTableLBA), () => {
                PathTable = s.SerializeObject<ISO9960_Sector<ISO9960_PathTable>>(PathTable, name: nameof(PathTable));
            });
            if (Directories == null) {
                Directories = new ISO9960_Sector<ISO9960_Directory>[PathTable.Object.Entries.Length];
                for (int i = 0; i < Directories.Length; i++) {
                    var entry = PathTable.Object.Entries[i];
                    s.DoAt(LBAToPointer(entry.ExtentLBA), () => {
                        Directories[i] = s.SerializeObject<ISO9960_Sector<ISO9960_Directory>>(Directories[i], name: $"{nameof(Directories)}[{i}]");
                    });
                }
            }
        }

        public Pointer LBAToPointer(uint lba) {
            return Offset + lba * SectorSize;
        }
    }
}