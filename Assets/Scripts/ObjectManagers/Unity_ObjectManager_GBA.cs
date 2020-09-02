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
        }

        public GraphicsData[] GraphicsDatas { get; }

        public override void InitLinkGroups(IList<Unity_Object> objects)
        {
            var usesLinks = Context.Settings.EngineVersion > EngineVersion.GBA_BatmanVengeance && Context.Settings.EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow;

            if (!usesLinks)
                return;

            var eventList = objects.Cast<Unity_Object_GBA>().Select(x => new
            {
                Data = x,
                Links = new int[]
                {
                    x.Actor.Link_0,
                    x.Actor.Link_1,
                    x.Actor.Link_2,
                    x.Actor.Link_3,
                }
            }).ToArray();

            for (int i = 0; i < eventList.Length; i++)
            {
                var data = eventList[i].Data;
                var links = eventList[i].Links;

                // Ignore already assigned ones
                if (data.EditorLinkGroup != 0)
                    continue;

                // No link
                if (links.All(x => x == 0xFF))
                {
                    data.EditorLinkGroup = 0;
                }
                // Link
                else
                {
                    data.EditorLinkGroup = i + 1;

                    foreach (var link in links)
                    {
                        if (link == 0xFF)
                            continue;

                        if (link >= eventList.Length)
                        {
                            Debug.LogWarning("Link ID " + link + " was too high (event count: " + eventList.Length);
                            continue;
                        }

                        eventList[link].Data.EditorLinkGroup = i + 1;

                        foreach (var linkedObj in eventList.Where(x => x.Data.EditorLinkGroup == 0 && x.Links.Contains(link)))
                            linkedObj.Data.EditorLinkGroup = i + 1;
                    }
                }
            }
        }

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