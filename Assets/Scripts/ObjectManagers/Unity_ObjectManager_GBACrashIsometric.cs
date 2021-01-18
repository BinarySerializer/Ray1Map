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

        public override void InitObjects(Unity_Level level)
        {
            var objects = level.EventData.Cast<Unity_Object_GBACrashIsometric>().ToArray();

            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];

                obj.Height = 0; // TODO: Set based on the height of the current tile

                var prevObjIndex = i - 1;
                var prevObj = objects[prevObjIndex];

                if (0 < i && obj.Object.XPos.Value == prevObj.Object.XPos.Value && obj.Object.YPos.Value == prevObj.Object.YPos.Value)
                {
                    while (true)
                    {
                        obj.Object.XPos.Value += 0x100;
                        obj.Object.YPos.Value += 0x100;
                        obj.Height += 0x1000;

                        if (prevObj.Object.XPos.Value != objects[prevObjIndex - 1].Object.XPos.Value || prevObj.Object.YPos.Value != objects[prevObjIndex - 1].Object.YPos.Value)
                            break;

                        prevObjIndex--;
                        prevObj = objects[prevObjIndex];
                    }
                }
            }
        }

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

                            // Center the frame
                            XPosition = - (int)(AnimFrames[x].rect.width / 2),
                            YPosition = - (int)(AnimFrames[x].rect.height / 2)
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