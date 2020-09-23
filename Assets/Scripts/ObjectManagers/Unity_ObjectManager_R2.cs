using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_R2 : Unity_ObjectManager
    {
        public Unity_ObjectManager_R2(Context context, ushort[] linkTable, AnimGroup[] animGroups, Sprite[] sprites, R1_ImageDescriptor[] imageDescriptors, R1_ZDCData[] zdc) : base(context)
        {
            LinkTable = linkTable;
            AnimGroups = animGroups;
            Sprites = sprites;
            ImageDescriptors = imageDescriptors;
            ZDC = zdc;
        }

        public AnimGroup[] AnimGroups { get; }
        public Sprite[] Sprites { get; }
        public R1_ImageDescriptor[] ImageDescriptors { get; }
        public R1_ZDCData[] ZDC { get; }

        public ushort[] LinkTable { get; }

        public override void InitLinkGroups(IList<Unity_Object> objects)
        {
            int currentId = 1;

            for (int i = 0; i < LinkTable.Length; i++)
            {
                if (i >= objects.Count)
                    break;

                // No link
                if (LinkTable[i] == i)
                {
                    objects[i].EditorLinkGroup = 0;
                }
                else
                {
                    // Ignore already assigned ones
                    if (objects[i].EditorLinkGroup != 0)
                        continue;

                    // Link found, loop through everyone on the link chain
                    int nextEvent = LinkTable[i];
                    objects[i].EditorLinkGroup = currentId;
                    int prevEvent = i;
                    while (nextEvent != i && nextEvent != prevEvent)
                    {
                        prevEvent = nextEvent;
                        objects[nextEvent].EditorLinkGroup = currentId;
                        nextEvent = LinkTable[nextEvent];
                    }
                    currentId++;
                }
            }
        }

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.Cast<Unity_Object_R2>().FindItem(x => x.EventData.EventType == R1_R2EventType.RaymanPosition);

        [Obsolete]
        public override string[] LegacyDESNames => AnimGroups.Select(x => x.Pointer?.ToString() ?? "N/A").ToArray();
        [Obsolete]
        public override string[] LegacyETANames => LegacyDESNames;

        public class AnimGroup
        {
            public AnimGroup(Pointer pointer, R1_EventState[][] eta, Unity_ObjAnimation[] animations, string filePath)
            {
                Pointer = pointer;
                ETA = eta;
                Animations = animations ?? new Unity_ObjAnimation[0];
                FilePath = filePath;
            }

            public Pointer Pointer { get; }

            public R1_EventState[][] ETA { get; }

            public Unity_ObjAnimation[] Animations { get; }

            public string FilePath { get; }
        }
    }
}