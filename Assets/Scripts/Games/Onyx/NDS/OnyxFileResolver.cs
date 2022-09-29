using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Nintendo.NDS;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class OnyxFileResolver
    {
        public const string Key = nameof(OnyxFileResolver);

        public OnyxFileResolver(ROMHeader ndsRom)
        {
            NDS_ROM = ndsRom;
            NDS_FileTable = ndsRom.CreateFileTable();

            foreach (var entry in NDS_FileTable)
                ndsRom.Offset.File.AddRegion(entry.Value.StartPointer.FileOffset, entry.Value.Length, entry.Key);

            // TODO: Define file extensions as well?
            // TODO: Other versions of the engine have more types such as for 2D levels, actors etc.
            NDS_FileTypes = new Dictionary<string, Type>
            {
                ["----"] = null,
                ["BMP "] = typeof(BitmapFile),
                ["PLTT"] = typeof(PaletteFile),
                ["FONT"] = null, // TODO: Font
                ["LANG"] = null, // TODO: Localization
                ["LV3D"] = null, // TODO: Level3D
                ["PSMG"] = null, // TODO: SceneManager
                ["MESH"] = null, // TODO: Mesh
                ["GEOM"] = null, // TODO: Geometry
                ["TEXL"] = typeof(TextureFile),
                ["SSCE"] = null, // TODO: ?
                ["SCMG"] = null, // TODO: ?
                ["FX3D"] = null, // TODO: ?
                ["AMBM"] = null, // TODO: ?
                ["EPAT"] = null, // TODO: PhysicsMaterial
                ["MNAM"] = null, // TODO: Animation3D
                ["MDAT"] = null, // TODO: Animation3D_Data
                ["AOBJ"] = typeof(PuppetFile),
                ["TBLS"] = typeof(SpriteTileSetFile),
                ["TLKT"] = null, // TODO: ?
                ["TKNV"] = typeof(TileKitFile),
                ["PF2D"] = null, // TODO: ?
                ["PFNV"] = null, // TODO: PlayField
                ["SOUN"] = null, // TODO: ?
                ["TXFX"] = null, // TODO: ?

                ["GO3D"] = null, // TODO: Template (unused, leftover from their editor)
            };
            NDS_FileTypeIDs = NDS_FileTypes.Where(x => x.Value != null).ToDictionary(x => x.Value, x => x.Key);
        }
        private Dictionary<string, FAT_Entry> NDS_FileTable { get; }

        public ROMHeader NDS_ROM { get; }
        public Dictionary<string, Type> NDS_FileTypes { get; }
        public Dictionary<Type, string> NDS_FileTypeIDs { get; }

        public IEnumerable<string> NDS_EnumerateFilePaths() => NDS_FileTable.Keys;

        public FilePointer NDS_GetFilePointer(string filePath)
        {
            FAT_Entry file = NDS_FileTable.TryGetValue(filePath, out FAT_Entry f) ? f : null;

            if (file == null)
                return null;

            return new FilePointer(file.StartPointer, file.Length);
        }

        public OnyxFile NDS_DeserializeFile(BinaryDeserializer s, string filePath)
        {
            FilePointer filePointer = NDS_GetFilePointer(filePath);

            OnyxFile file = null;

            s.DoAt(filePointer?.Pointer, () =>
            {
                string formatID = s.SerializeString(default, length: 4, name: "FormatID").Reverse();
                Type type = NDS_FileTypes.TryGetValue(formatID, out Type t) ? t : null;

                if (type == null)
                {
                    file = null;
                    s.SystemLogger?.LogWarning("File format {0} does not have a matching type", formatID);
                    return;
                }

                file = (OnyxFile)Activator.CreateInstance(type);
                
                s.Goto(filePointer!.Pointer);
                file.Init(s.CurrentPointer);
                file.Serialize(s);
            });

            return file;
        }

        public class FilePointer
        {
            public FilePointer(Pointer pointer, long length)
            {
                Pointer = pointer;
                Length = length;
            }

            public Pointer Pointer { get; }
            public long Length { get; }
        }
    }
}