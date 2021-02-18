using System;
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

        public override void InitObjects(Unity_Level level)
        {
            foreach (var obj in level.EventData.Cast<Unity_Object_GBARRR>().Where(x => x.Object.ObjectType == GBARRR_ObjectType.Gate || x.Object.ObjectType == GBARRR_ObjectType.DoorTrigger))
                obj.LinkIndex = level.EventData.Cast<Unity_Object_GBARRR>().FindItemIndex(x => x.Object.RuntimeStateIndex == obj.Object.RuntimeStateIndex && x != obj && (x.Object.ObjectType == GBARRR_ObjectType.Gate || x.Object.ObjectType == GBARRR_ObjectType.DoorTrigger));
        }

        public override int InitLinkGroups(IList<Unity_Object> objects)
        {
            var links = new Dictionary<int, List<Unity_Object_GBARRR>>();

            foreach (var obj in objects.Cast<Unity_Object_GBARRR>())
            {
                if (obj.Object.LinkGroup == 0)
                    continue;

                if (!links.ContainsKey(obj.Object.LinkGroup))
                    links[obj.Object.LinkGroup] = new List<Unity_Object_GBARRR>();

                links[obj.Object.LinkGroup].Add(obj);
            }

            foreach (var l in links.Where(x => x.Value.Count > 1))
                foreach (var obj in l.Value)
                    obj.EditorLinkGroup = l.Key;

            return objects.Any() ? objects.Max(x => x.EditorLinkGroup) + 1 : 1;
        }

        public GraphicsData[][] GraphicsDatas { get; }

        public class GraphicsData
        {
            public GraphicsData(Func<Sprite[]> animFrameFunc, byte animSpeed, int blockIndex)
            {
                AnimFrameFunc = animFrameFunc;
                AnimSpeed = animSpeed;
                BlockIndex = blockIndex;
            }

            private Sprite[] Frames;
            private Unity_ObjAnimation Anim;
            protected Func<Sprite[]> AnimFrameFunc { get; }

            public Sprite[] AnimFrames => Frames ?? (Frames = AnimFrameFunc());

            public Unity_ObjAnimation Animation => Anim ?? (Anim = new Unity_ObjAnimation()
            {
                Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                {
                    new Unity_ObjAnimationPart()
                    {
                        ImageIndex = x
                    }
                })).ToArray()
            });
            public byte AnimSpeed { get; }
            public int BlockIndex { get; }
        }

        public override string[] LegacyDESNames => GraphicsDatas.Select((x,i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}