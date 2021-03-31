using System;

using System.Linq;
using BinarySerializer;
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

                if (obj is Unity_Object_GBAVVIsometric_TargetObjTarget)
                    continue;

                float yScaled = obj.YPos * scale;
                float xScaled = obj.XPos * scale;
                var collY = Mathf.FloorToInt(yScaled);
                var collX = Mathf.FloorToInt(xScaled);
                /*var collY = (((long)obj.YPos.Value) * 0x15555556) >> 0x2a;
                var collX = (((long)obj.XPos.Value) * 0x15555556) >> 0x2a;*/
                var tile = MapData.CollisionTiles[MapData.CollisionMap[collY * MapData.CollisionWidth + collX]];
                obj.Height = (tile.Height - minHeight) / scale;

                int baseType = (int)Unity_IsometricCollisionTile.CollisionType.GBAVV_Solid_0;
                var type = (Unity_IsometricCollisionTile.CollisionType)(baseType+GetCommonCollisionTypeFunc(tile.TypeIndex));

                switch (type) {
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_12:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_17:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_19:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_21:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_22:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_24:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_26:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_28:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_32:
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Corner_35:
                        // Diagonal
                        bool isHigh = false;
                        switch (tile.Shape % 4) {
                            case 0: // bottom is higher (high x & y)
                                isHigh = (yScaled - collY) + (xScaled - collX) >= 1f;
                                break;
                            case 1: // left is higher (high y, low x)
                                isHigh = (yScaled - collY) + (1f - (xScaled - collX)) >= 1f;
                                break;
                            case 2: // top is higher (low x & y)
                                isHigh = (1f - (yScaled - collY)) + (1f - (xScaled - collX)) >= 1f;
                                break;
                            case 3: // right is higher (low y, high x)
                                isHigh = (1f - (yScaled - collY)) + (xScaled - collX) >= 1f;
                                break;
                        }
                        if(isHigh)
                            obj.Height += MapData.CollisionTypes[tile.TypeIndex].AdditionalHeight / scale;
                        break;
                    case Unity_IsometricCollisionTile.CollisionType.GBAVV_Ramp_1:
                        // Ramp
                        float x = (xScaled - collX);
                        float y = (yScaled - collY);
                        float rampHeight = 0.1875f;
                        switch (tile.Shape % 4) {
                            case 0: // down left is higher (high y)
                                obj.Height += (1-Mathf.Sqrt(1 - y*y)) * rampHeight / scale;
                                break;
                            case 1: // up left is higher (low x)
                                obj.Height += (1 - Mathf.Sqrt(1 - (1 - x) * (1 - x))) * rampHeight / scale;
                                break;
                            case 2: // up right is higher (low y)
                                obj.Height += (1 - Mathf.Sqrt(1 - (1 - y) * (1 - y))) * rampHeight / scale;
                                break;
                            case 3: // down right is higher (high x)
                                obj.Height += (1 - Mathf.Sqrt(1 - x*x)) * rampHeight / scale;
                                break;
                        }
                        break;
                    default:
                        break;
                }

                if (obj is Unity_Object_GBAVVIsometric_TargetObj t)
                    objects[t.LinkIndex].Height = t.Height;

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

                var x = crashAnim.XPos;
                var y = crashAnim.YPos;

                // Fix for the exit animation
                if (y.Value == 0)
                    y = x;

                Animation = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(frameIndex => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = frameIndex,

                            // Center the frame
                            XPosition = (int)(x * 16f),
                            YPosition = (int)(y * 16f),
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