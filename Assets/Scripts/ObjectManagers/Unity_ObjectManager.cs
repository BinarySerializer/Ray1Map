using System;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    public class Unity_ObjectManager
    {
        public Unity_ObjectManager(Context context)
        {
            Context = context;
        }

        public Context Context { get; }

        public virtual string[] GetAvailableObjects => new string[0];
        public virtual Unity_Object CreateObject(int index) => null;

        public virtual void InitR1LinkGroups(IList<Unity_Object> objects) { }
        protected void InitR1LinkGroups(IList<Unity_Object> objects, ushort[] linkTable)
        {
            int currentId = 1;

            for (int i = 0; i < objects.Count; i++)
            {
                if (i >= objects.Count)
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

        public virtual bool UpdateFromMemory(ref Context gameMemoryContext) => false;
    }
}