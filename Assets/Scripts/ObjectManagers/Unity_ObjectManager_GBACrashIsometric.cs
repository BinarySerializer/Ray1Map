using R1Engine.Serialize;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBACrashIsometric : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBACrashIsometric(Context context, GraphicsData[] graphicsDatas) : base(context)
        {
            GraphicsDatas = graphicsDatas;
        }

        public GraphicsData[] GraphicsDatas { get; }

        public class GraphicsData
        {
            public GraphicsData(Sprite[] animFrames, byte animSpeed)
            {
                AnimFrames = animFrames;
                AnimSpeed = animSpeed;

                Animation = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = 0,
                            YPosition = 0,
                        }
                    })).ToArray()
                };
            }

            public Sprite[] AnimFrames { get; }
            public byte AnimSpeed { get; }

            public Unity_ObjAnimation Animation { get; }
        }

        public override string[] LegacyDESNames => GraphicsDatas.Select((x,i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}