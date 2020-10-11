using System;
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

        public uint? GetFileLBA(string filePath, bool throwOnError)
        {
            if (filePath == null) 
                throw new ArgumentNullException(nameof(filePath));

            // Get the paths
            var paths = filePath.Trim('\\').Split('\\');

            if (!paths.Any())
            {
                if (throwOnError)
                    throw new Exception("The file path can't be empty");
                else
                    return null;
            }

            // Default to root
            var dirIndex = 0;
            uint lba = PathTable.Object.Entries.First().ExtentLBA;

            // Get the directory LBA
            foreach (var dir in paths.Take(paths.Length - 1))
            {
                dirIndex = PathTable.Object.Entries.FindItemIndex(x => x.ParentDirectoryIndex == dirIndex + 1 && x.DirectoryIdentifier == dir);

                if (dirIndex == -1)
                {
                    if (throwOnError)
                        throw new Exception($"Directory {dir} not found");
                    else
                        return null;
                }

                lba = PathTable.Object.Entries[dirIndex].ExtentLBA;
            }

            // Get the directory records to find the file
            var dirRecords = Directories.FirstOrDefault(x => x.Object.Entries.First().ExtentLBA == lba)?.Object.Entries;

            if (dirRecords == null)
            {
                if (throwOnError)
                    throw new Exception($"Directory not found for LBA {lba}");
                else
                    return null;
            }

            // Find the file
            var fileName = paths.Last();

            var file = dirRecords.FirstOrDefault(x => x.FileIdentifier == fileName && !x.FileFlags.HasFlag(ISO9960_DirectoryRecord.RecordFileFlags.Directory));

            if (file == null)
            {
                if (throwOnError)
                    throw new Exception($"File {fileName} not found in directory with LBA {lba}");
                else
                    return null;
            }

            return file.ExtentLBA;
        }
    }
}