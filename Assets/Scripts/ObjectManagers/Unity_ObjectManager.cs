using System;
using System.Collections.Generic;
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

        public virtual void InitEvents(Unity_Level level) { }
        public virtual Unity_Object GetMainObject(IList<Unity_Object> objects) => null;

        [Obsolete]
        public virtual string[] LegacyDESNames => new string[0];
        [Obsolete]
        public virtual string[] LegacyETANames => new string[0];

        public virtual bool UpdateFromMemory(Context gameMemoryContext) => false;
    }
}