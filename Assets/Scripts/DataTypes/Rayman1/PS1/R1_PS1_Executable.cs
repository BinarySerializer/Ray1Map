using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class R1_PS1_Executable : R1Serializable
    {
        public R1_ZDCEntry[] TypeZDC { get; set; }
        public R1_ZDCData[] ZDCData { get; set; }
        public R1_EventFlags[] EventFlags { get; set; }

        public R1_PS1_FileTableEntry[] FileTable { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var manager = (R1_PS1BaseManager)s.GameSettings.GetGameManager;

            if (manager.TypeZDCOffset != null)
                TypeZDC = s.DoAt(new Pointer(manager.TypeZDCOffset.Value, Offset.file), () => s.SerializeObjectArray<R1_ZDCEntry>(TypeZDC, manager.TypeZDCCount, name: nameof(TypeZDC)));

            if (manager.ZDCDataOffset != null)
                ZDCData = s.DoAt(new Pointer(manager.ZDCDataOffset.Value, Offset.file), () => s.SerializeObjectArray<R1_ZDCData>(ZDCData, manager.ZDCDataCount, name: nameof(ZDCData)));

            if (manager.EventFlagsOffset != null)
            {
                if (s.GameSettings.EngineVersion == EngineVersion.R1_Saturn)
                    EventFlags = s.DoAt(new Pointer(manager.EventFlagsOffset.Value, Offset.file), 
                        () => s.SerializeArray<int>(EventFlags.Select(x => BitHelpers.ReverseBits((int)x)).ToArray(), manager.EventFlagsCount, name: nameof(EventFlags))).Select(BitHelpers.ReverseBits).Select(x => (R1_EventFlags)x).ToArray();
                else
                    EventFlags = s.DoAt(new Pointer(manager.EventFlagsOffset.Value, Offset.file), 
                        () => s.SerializeArray<R1_EventFlags>(EventFlags, manager.EventFlagsCount, name: nameof(EventFlags)));
            }

            var fileTableInfos = manager.FileTableInfos;

            if (FileTable == null)
                FileTable = new R1_PS1_FileTableEntry[fileTableInfos.Sum(x => x.Count)];

            var index = 0;
            foreach (var info in fileTableInfos)
            {
                s.DoAt(new Pointer(info.Offset, Offset.file), () =>
                {
                    for (int i = 0; i < info.Count; i++)
                    {
                        FileTable[index] = s.SerializeObject<R1_PS1_FileTableEntry>(FileTable[index], name: $"{nameof(FileTable)}_{info.Name}[{i}]");
                        index++;
                    }
                });
            }
        }
    }
}