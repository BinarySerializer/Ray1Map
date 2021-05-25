using System.Linq;
using R1Engine;

namespace BinarySerializer.Ray1
{
    public class PS1_Executable : BinarySerializable
    {
        public ZDCEntry[] TypeZDC { get; set; }
        public ZDCData[] ZDCData { get; set; }
        public ObjTypeFlags[] EventFlags { get; set; }

        public WorldInfo[] WorldInfo { get; set; }

        public byte[][] LevelBackgroundIndexTable { get; set; }

        public PS1_FileTableEntry[] FileTable { get; set; }

        public RGBA5551Color[] Saturn_Palettes { get; set; }
        public string[][] Saturn_FNDFileTable { get; set; }
        public string[][] Saturn_FNDSPFileTable { get; set; }
        public byte[][] Saturn_FNDIndexTable { get; set; }

        public int GetFileTypeIndex(R1_PS1BaseManager manager, PS1_FileType type) => FileTable.FindItemIndex(x => x.Offset.AbsoluteOffset == manager.FileTableInfos.FirstOrDefault(t => t.FileType == type)?.Offset);

        public override void SerializeImpl(SerializerObject s)
        {
            var manager = (R1_PS1BaseManager)s.GetR1Settings().GetGameManager;

            if (manager.TypeZDCOffset != null)
                TypeZDC = s.DoAt(new Pointer(manager.TypeZDCOffset.Value, Offset.File), () => s.SerializeObjectArray<ZDCEntry>(TypeZDC, manager.TypeZDCCount, name: nameof(TypeZDC)));

            if (manager.ZDCDataOffset != null)
                ZDCData = s.DoAt(new Pointer(manager.ZDCDataOffset.Value, Offset.File), () => s.SerializeObjectArray<ZDCData>(ZDCData, manager.ZDCDataCount, name: nameof(ZDCData)));

            if (manager.EventFlagsOffset != null)
            {
                if (s.GetR1Settings().EngineVersion == EngineVersion.R1_Saturn)
                    EventFlags = s.DoAt(new Pointer(manager.EventFlagsOffset.Value, Offset.File), 
                        () => s.SerializeArray<int>(EventFlags?.Select(x => BitHelpers.ReverseBits((int)x)).ToArray(), manager.EventFlagsCount, name: nameof(EventFlags))).Select(BitHelpers.ReverseBits).Select(x => (ObjTypeFlags)x).ToArray();
                else
                    EventFlags = s.DoAt(new Pointer(manager.EventFlagsOffset.Value, Offset.File), 
                        () => s.SerializeArray<ObjTypeFlags>(EventFlags, manager.EventFlagsCount, name: nameof(EventFlags)));
            }

            if (manager.WorldInfoOffset != null)
                WorldInfo = s.DoAt(new Pointer(manager.WorldInfoOffset.Value, Offset.File), () => s.SerializeObjectArray<WorldInfo>(WorldInfo, 24, name: nameof(WorldInfo)));

            if (manager.LevelBackgroundIndexTableOffset != null)
            {
                if (LevelBackgroundIndexTable == null)
                    LevelBackgroundIndexTable = new byte[6][];

                s.DoAt(new Pointer(manager.LevelBackgroundIndexTableOffset.Value, Offset.File), () =>
                {
                    for (int i = 0; i < LevelBackgroundIndexTable.Length; i++)
                        LevelBackgroundIndexTable[i] = s.SerializeArray<byte>(LevelBackgroundIndexTable[i], 30, name: $"{nameof(LevelBackgroundIndexTable)}[{i}]");
                });
            }

            var fileTableInfos = manager.FileTableInfos;

            if (FileTable == null)
                FileTable = new PS1_FileTableEntry[fileTableInfos.Sum(x => x.Count)];

            var index = 0;
            foreach (var info in fileTableInfos)
            {
                s.DoAt(new Pointer(info.Offset, Offset.File), () =>
                {
                    for (int i = 0; i < info.Count; i++)
                    {
                        FileTable[index] = s.SerializeObject<PS1_FileTableEntry>(FileTable[index], name: $"{nameof(FileTable)}_{info.FileType}[{i}]");
                        index++;
                    }
                });
            }

            if (s.GetR1Settings().EngineVersion == EngineVersion.R1_Saturn)
            {
                var saturnManager = (R1_Saturn_Manager)manager;

                Saturn_Palettes = s.DoAt(new Pointer(saturnManager.GetPalOffset, Offset.File), () => s.SerializeObjectArray<RGBA5551Color>(Saturn_Palettes, 25 * 256 * 2, name: nameof(Saturn_Palettes)));

                if (Saturn_FNDFileTable == null)
                    Saturn_FNDFileTable = new string[6][];

                s.DoAt(new Pointer(saturnManager.GetFndFileTableOffset, Offset.File), () =>
                {
                    for (int i = 0; i < Saturn_FNDFileTable.Length; i++)
                        Saturn_FNDFileTable[i] = s.SerializeStringArray(Saturn_FNDFileTable[i], 10, 12, name: $"{nameof(Saturn_FNDFileTable)}[{i}]");
                });

                if (Saturn_FNDSPFileTable == null)
                    Saturn_FNDSPFileTable = new string[6][];

                s.DoAt(new Pointer(saturnManager.GetFndSPFileTableOffset, Offset.File), () =>
                {
                    for (int i = 0; i < Saturn_FNDSPFileTable.Length; i++)
                        Saturn_FNDSPFileTable[i] = s.SerializeStringArray(Saturn_FNDSPFileTable[i], 5, 12, name: $"{nameof(Saturn_FNDSPFileTable)}[{i}]");
                });

                if (Saturn_FNDIndexTable == null)
                    Saturn_FNDIndexTable = new byte[7][];

                s.DoAt(new Pointer(saturnManager.GetFndIndexTableOffset, Offset.File), () =>
                {
                    for (int i = 0; i < Saturn_FNDIndexTable.Length; i++)
                        Saturn_FNDIndexTable[i] = s.SerializeArray<byte>(Saturn_FNDIndexTable[i], 25, name: $"{nameof(Saturn_FNDIndexTable)}[{i}]");
                });
            }
        }
    }
}