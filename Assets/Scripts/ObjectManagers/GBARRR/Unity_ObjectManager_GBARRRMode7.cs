using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBARRRMode7 : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBARRRMode7(Context context, GraphicsData[] graphicsDatas) : base(context)
        {
            GraphicsDatas = graphicsDatas;
        }

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.FindItem(x => (x as Unity_Object_GBARRRMode7)?.IsRayman ?? false);

        public GraphicsData[] GraphicsDatas { get; }

        public class GraphicsData
        {
            public GraphicsData(Sprite[] animFrames, byte animSpeed, Vector2Int[] positions)
            {
                AnimFrames = animFrames;
                AnimSpeed = animSpeed;
                Positions = positions;

                Animation = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = Positions[x].x,
                            YPosition = Positions[x].y,
                        }
                    })).ToArray()
                };
            }

            public Sprite[] AnimFrames { get; }
            public byte AnimSpeed { get; }
            public Vector2Int[] Positions { get; }

            public Unity_ObjAnimation Animation { get; }
        }

        public override string[] LegacyDESNames => GraphicsDatas.Select((x,i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}