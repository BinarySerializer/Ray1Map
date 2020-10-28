using System.Collections.Generic;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBARRR : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBARRR(Context context, GraphicsData[][] graphicsDatas) : base(context)
        {
            GraphicsDatas = graphicsDatas;
        }

        public override int InitLinkGroups(IList<Unity_Object> objects)
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
                    obj.EditorLinkGroup = l.Key;

            return objects.Any() ? objects.Max(x => x.EditorLinkGroup) + 1 : 1;
        }

        public GraphicsData[][] GraphicsDatas { get; }

        public class GraphicsData
        {
            public GraphicsData(Sprite[] animFrames, byte animSpeed, int blockIndex)
            {
                AnimFrames = animFrames;
                AnimSpeed = animSpeed;
                BlockIndex = blockIndex;
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

            public Sprite[] AnimFrames { get; }
            public Unity_ObjAnimation Animation { get; }
            public byte AnimSpeed { get; }
            public int BlockIndex { get; }
        }

        public override string[] LegacyDESNames => GraphicsDatas.Select((x,i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}