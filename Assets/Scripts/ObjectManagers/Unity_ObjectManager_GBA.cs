using System;
using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBA : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBA(Context context, GraphicsData[] graphicsDatas) : base(context)
        {
            GraphicsDatas = graphicsDatas;
            if (GraphicsDatas != null) {
                for (int i = 0; i < GraphicsDatas.Length; i++) {
                    GraphicsDataLookup[GraphicsDatas[i]?.Index ?? 0] = i;
                }
            }
        }

        public GraphicsData[] GraphicsDatas { get; }
        public Dictionary<int, int> GraphicsDataLookup { get; } = new Dictionary<int, int>();

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.Cast<Unity_Object_GBA>().FindItem(x => x.Actor.ActorID == 0);

        [Obsolete]
        public override string[] LegacyDESNames => GraphicsDatas.Select(x => x.Index.ToString()).ToArray();
        [Obsolete]
        public override string[] LegacyETANames => LegacyDESNames;

        public class GraphicsData
        {
            public GraphicsData(int index, GBA_ActorState[] states, Unity_ObjGraphics graphics)
            {
                Index = index;
                States = states;
                Graphics = graphics;
            }

            public int Index { get; }

            public GBA_ActorState[] States { get; }

            public Unity_ObjGraphics Graphics { get; }
        }
    }
}