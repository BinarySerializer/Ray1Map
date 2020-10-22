using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBARRR : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBARRR(Context context, GraphicsData[] graphicsDatas) : base(context)
        {
            GraphicsDatas = graphicsDatas;

            for (int i = 0; i < GraphicsDatas.Length; i++)
                GraphicsDataLookup[GraphicsDatas[i]?.GraphicsOffset ?? 0] = i;
        }

        public override int InitR1LinkGroups(IList<Unity_Object> objects)
        {
            var links = new Dictionary<int, List<Unity_Object_GBARRR>>();

            foreach (var obj in objects.Cast<Unity_Object_GBARRR>())
            {
                if (obj.Actor.LinkGroup == 0)
                    continue;

                if (!links.ContainsKey(obj.Actor.LinkGroup))
                    links[obj.Actor.LinkGroup] = new List<Unity_Object_GBARRR>();

                links[obj.Actor.LinkGroup].Add(obj);
            }

            foreach (var l in links.Where(x => x.Value.Count > 1))
                foreach (var obj in l.Value)
                    obj.R1_EditorLinkGroup = l.Key;

            return objects.Max(x => x.R1_EditorLinkGroup) + 1;
        }

        public GraphicsData[] GraphicsDatas { get; }
        public Dictionary<uint, int> GraphicsDataLookup { get; } = new Dictionary<uint, int>();

        public class GraphicsData
        {
            public GraphicsData(uint graphicsOffset, Sprite[] animFrames)
            {
                GraphicsOffset = graphicsOffset;
                AnimFrames = animFrames;
                Animation = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, animFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x
                        }
                    })).ToArray()
                };
            }

            public uint GraphicsOffset { get; }
            public Sprite[] AnimFrames { get; }
            public Unity_ObjAnimation Animation { get; }
        }
    }
}