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