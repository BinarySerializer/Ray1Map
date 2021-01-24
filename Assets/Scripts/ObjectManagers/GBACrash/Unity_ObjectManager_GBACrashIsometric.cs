using R1Engine.Serialize;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBACrashIsometric : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBACrashIsometric(Context context, GraphicsData[] graphicsDatas, GBACrash_Isometric_LevelInfo level) : base(context)
        {
            GraphicsDatas = graphicsDatas;
            Level = level;
        }

        public GraphicsData[] GraphicsDatas { get; }
        public GBACrash_Isometric_LevelInfo Level { get; }

        public override void InitObjects(Unity_Level level)
        {
            var objects = level.EventData.Cast<Unity_Object_BaseGBACrashIsometric>().ToArray();
            float minHeight = Mathf.Min(0, Level.CollisionMap.Min(c => Level.CollisionTiles[c].Height.AsFloat));
            const float scale = 64f / 12;

            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];

                obj.UpdateAnimIndex();

                var collY = Mathf.FloorToInt(obj.YPos * scale);
                var collX = Mathf.FloorToInt(obj.XPos * scale);
                /*var collY = (((long)obj.Object.YPos.Value) * 0x15555556) >> 0x2a;
                var collX = (((long)obj.Object.XPos.Value) * 0x15555556) >> 0x2a;*/
                obj.Height = (Level.CollisionTiles[Level.CollisionMap[collY * Level.CollisionWidth + collX]].Height - minHeight) / scale;

                if (i == 0 || !(obj is Unity_Object_GBACrashIsometric_Obj))
                    continue;

                var prevObjIndex = i - 1;
                var prevObj = objects[prevObjIndex];

                if (obj.XPos.Value == prevObj.XPos.Value && obj.YPos.Value == prevObj.YPos.Value)
                {
                    while (true)
                    {
                        obj.XPos.Value += 0x100;
                        obj.YPos.Value += 0x100;
                        obj.Height += ((0x1000 / (float)(1 << 16))) / scale;

                        if (prevObj.XPos.Value != objects[prevObjIndex - 1].XPos.Value || prevObj.YPos.Value != objects[prevObjIndex - 1].YPos.Value)
                            break;

                        prevObjIndex--;
                        prevObj = objects[prevObjIndex];
                    }
                }
            }

            //foreach (Unity_Object_BaseGBACrashIsometric obj in objects)
            //{
            //    obj.XPos.Value += obj.GraphicsData?.CrashAnim?.XPos?.Value / 2 ?? 0;
            //    obj.YPos.Value += obj.GraphicsData?.CrashAnim?.YPos?.Value / 2 ?? 0;
            //}
        }

        public class GraphicsData
        {
            public GraphicsData(Sprite[] animFrames, byte animSpeed, GBACrash_Isometric_Animation crashAnim)
            {
                AnimFrames = animFrames;
                AnimSpeed = animSpeed;
                CrashAnim = crashAnim;

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
            public GBACrash_Isometric_Animation CrashAnim { get; }

            public Unity_ObjAnimation Animation { get; }
        }

        public override string[] LegacyDESNames => GraphicsDatas.Select((x,i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}