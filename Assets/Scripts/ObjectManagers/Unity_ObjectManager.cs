using System;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager
    {
        public Unity_ObjectManager(Context context)
        {
            Context = context;
        }

        public Context Context { get; }

        public virtual int MaxObjectCount => Byte.MaxValue;
        public virtual string[] GetAvailableObjects => new string[0];
        public virtual Unity_Object CreateObject(int index) => null;

        public virtual int InitR1LinkGroups(IList<Unity_Object> objects) { return 0; }
        protected int InitR1LinkGroups(IList<Unity_Object> objects, ushort[] linkTable)
        {
            int currentId = 1;

            for (int i = 0; i < objects.Count; i++)
            {
                if (i >= linkTable.Length)
                    break;

                // No link
                if (linkTable[i] == i)
                {
                    objects[i].R1_EditorLinkGroup = 0;
                }
                else
                {
                    // Ignore already assigned ones
                    if (objects[i].R1_EditorLinkGroup != 0)
                        continue;

                    // Link found, loop through everyone on the link chain
                    int nextEvent = linkTable[i];
                    objects[i].R1_EditorLinkGroup = currentId;
                    int prevEvent = i;
                    while (nextEvent != i && nextEvent != prevEvent)
                    {
                        prevEvent = nextEvent;
                        objects[nextEvent].R1_EditorLinkGroup = currentId;
                        nextEvent = linkTable[nextEvent];
                    }
                    currentId++;
                }
            }

            return currentId;
        }
        public virtual void SaveLinkGroups(IList<Unity_Object> objects) { }
        protected ushort[] SaveR1LinkGroups(IList<Unity_Object> objects)
        {
            var linkTable = new ushort[objects.Count];

            List<int> alreadyChained = new List<int>();

            for (ushort i = 0; i < objects.Count; i++)
            {
                var obj = objects[i];
                
                // No link
                if (obj.R1_EditorLinkGroup == 0)
                {
                    linkTable[i] = i;
                }
                else
                {
                    // Skip if already chained
                    if (alreadyChained.Contains(i))
                        continue;

                    // Find all the events with the same linkId and store their indexes
                    List<ushort> indexesOfSameId = new List<ushort>();
                    int cur = obj.R1_EditorLinkGroup;
                    foreach (var e in objects.Where(e => e.R1_EditorLinkGroup == cur))
                    {
                        indexesOfSameId.Add((ushort)objects.IndexOf(e));
                        alreadyChained.Add(objects.IndexOf(e));
                    }

                    // Loop through and chain them
                    for (int j = 0; j < indexesOfSameId.Count; j++)
                    {
                        int next = j + 1;
                        if (next == indexesOfSameId.Count)
                            next = 0;

                        linkTable[indexesOfSameId[j]] = indexesOfSameId[next];
                    }
                }
            }

            return linkTable;
        }

        public virtual void InitObjects(Unity_Level level) { }
        public virtual Unity_Object GetMainObject(IList<Unity_Object> objects) => null;
        public virtual void SaveObjects(IList<Unity_Object> objects) { }

        [Obsolete]
        public virtual string[] LegacyDESNames => new string[0];
        [Obsolete]
        public virtual string[] LegacyETANames => new string[0];

        public bool UpdateFromMemory(ref Context gameMemoryContext)
        {
            const string memFileKey = "MemStream";

            // TODO: Dispose when we stop program?
            if (gameMemoryContext == null)
            {
                gameMemoryContext = new Context(LevelEditorData.CurrentSettings);

                try
                {
                    var file = new ProcessMemoryStreamFile(memFileKey, Settings.ProcessName, gameMemoryContext);

                    gameMemoryContext.AddFile(file);

                    var offset = file.StartPointer;
                    long baseStreamOffset;
                    var processBase = file.GetStream().GetProcessBaseAddress(Settings.ModuleName);

                    var s = gameMemoryContext.Deserializer;

                    if (Settings.IsGameBaseAPointer)
                    {
                        var basePtrPtr = offset + Settings.GameBasePointer;

                        if (Settings.FindPointerAutomatically)
                        {
                            try
                            {
                                basePtrPtr = file.GetPointerByName("MemBase"); // MemBase is the variable name in Dosbox.
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning($"Couldn't find pointer automatically ({ex.Message}), falling back on manual specification {basePtrPtr}");
                            }
                        }

                        // Get the base pointer
                        baseStreamOffset = file.GetStream().Is64BitProcess ? 
                            s.DoAt(basePtrPtr, () => s.Serialize<long>(default)) :
                            s.DoAt(basePtrPtr, () => s.SerializePointer(default)).AbsoluteOffset;
                    }
                    else
                    {
                        baseStreamOffset = Settings.GameBasePointer + processBase;
                    }

                    // TODO: Find better way to handle this
                    if (s.GameSettings.EngineVersion == EngineVersion.R1_PS1 ||
                        s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JP ||
                        s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol3 ||
                        s.GameSettings.EngineVersion == EngineVersion.R1_PS1_JPDemoVol6 ||
                        s.GameSettings.EngineVersion == EngineVersion.R2_PS1)
                        baseStreamOffset -= 0x80000000;

                    file.BaseStreamOffset = baseStreamOffset;

                    InitMemoryLoading(s, offset);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"{ex.Message}{Environment.NewLine}{ex}");
                    gameMemoryContext = null;
                }
            }

            if (gameMemoryContext != null)
                return DoMemoryLoading(gameMemoryContext, gameMemoryContext.GetFile(memFileKey).StartPointer);

            return false;
        }

        protected virtual void InitMemoryLoading(SerializerObject s, Pointer offset) { }
        protected virtual bool DoMemoryLoading(Context gameMemoryContext, Pointer offset) => false;

        public virtual bool IsObjectAlways(int index) => false;
    }
}
