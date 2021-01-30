using System;
using R1Engine.Serialize;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAVVIsometric : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAVVIsometric(Context context, GraphicsData[] graphicsDatas, GBAVV_Isometric_MapData mapData, Func<int, int> getCommonCollisionTypeFunc) : base(context)
        {
            GraphicsDatas = graphicsDatas;
            MapData = mapData;
            GetCommonCollisionTypeFunc = getCommonCollisionTypeFunc;
        }

        public GraphicsData[] GraphicsDatas { get; }
        public GBAVV_Isometric_MapData MapData { get; }
        public Func<int, int> GetCommonCollisionTypeFunc { get; }

        public override void InitObjects(Unity_Level level)
        {
            var objects = level.EventData.Cast<Unity_Object_BaseGBAVVIsometric>().ToArray();
            float minHeight = Mathf.Min(0, MapData.CollisionMap.Min(c => MapData.CollisionTiles[c].Height.AsFloat));
            const float scale = 64f / 12;

            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];

                obj.UpdateAnimIndex();

                var collY = Mathf.FloorToInt(obj.YPos * scale);
                var collX = Mathf.FloorToInt(obj.XPos * scale);
                /*var collY = (((long)obj.YPos.Value) * 0x15555556) >> 0x2a;
                var collX = (((long)obj.XPos.Value) * 0x15555556) >> 0x2a;*/
                obj.Height = (MapData.CollisionTiles[MapData.CollisionMap[collY * MapData.CollisionWidth + collX]].Height - minHeight) / scale;


                int baseType = (int)Unity_IsometricCollisionTile.CollisionType.GBAVV_0;
                // TODO: Access GBAVV_BaseManager.GetIsometricCollisionType(level, typeIndex) here so we can get the type, see GBAVV_BaseManager:601
                // Based on that determine if the block is diagonal
                // Then check the shape, and only then add the additional height if the object is on the right part of the block
                //obj.Height += MapData.CollisionTypes[MapData.CollisionTiles[MapData.CollisionMap[collY * MapData.CollisionWidth + collX]].TypeIndex].AdditionalHeight / scale;

                if (i == 0 || !(obj is Unity_Object_GBAVVIsometric_Obj))
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

            //foreach (Unity_Object_BaseGBAVVIsometric obj in objects)
            //{
            //    obj.XPos.Value += obj.GraphicsData?.CrashAnim?.XPos?.Value / 2 ?? 0;
            //    obj.YPos.Value += obj.GraphicsData?.CrashAnim?.YPos?.Value / 2 ?? 0;
            //}
        }

        public class GraphicsData
        {
            public GraphicsData(Sprite[] animFrames, byte animSpeed, GBAVV_Isometric_Animation crashAnim)
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
                            XPosition = (int)(crashAnim.XPos * 16f),
                            YPosition = (int)(crashAnim.YPos * 16f),
                            /*XPosition = - (int)(AnimFrames[x].rect.width / 2),
                            YPosition = - (int)(AnimFrames[x].rect.height / 2)*/
                        }
                    })).ToArray()
                };
            }

            public Sprite[] AnimFrames { get; }
            public byte AnimSpeed { get; }
            public GBAVV_Isometric_Animation CrashAnim { get; }

            public Unity_ObjAnimation Animation { get; }
        }

        public override string[] LegacyDESNames => GraphicsDatas.Select((x,i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}