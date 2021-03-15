using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBARRRMode7Unused : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBARRRMode7Unused(Context context, GraphicsDataGroup[] graphicsDatas) : base(context)
        {
            GraphicsDatas = graphicsDatas;
        }

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.FindItem(x => (x as Unity_Object_GBARRRMode7Unused)?.IsRayman ?? false);

        public GraphicsDataGroup[] GraphicsDatas { get; }

        public class GraphicsDataGroup
        {
            public GraphicsDataGroup(GraphicsData[] sprites, int blockIndex)
            {
                Sprites = sprites;
                BlockIndex = blockIndex;
            }

            public GraphicsData[] Sprites { get; }
            public int BlockIndex { get; }
        }

        public class GraphicsData
        {
            public GraphicsData(Sprite sprite)
            {
                AnimFrames = new Sprite[]
                {
                    sprite
                };

                Animation = new Unity_ObjAnimation()
                {
                    Frames = new Unity_ObjAnimationFrame[]
                    {
                        new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = 0
                            }
                        })
                    }
                };
            }

            public Sprite[] AnimFrames { get; }
            public Unity_ObjAnimation Animation { get; }
        }

        public override string[] LegacyDESNames => GraphicsDatas.Select(x => x.BlockIndex.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}