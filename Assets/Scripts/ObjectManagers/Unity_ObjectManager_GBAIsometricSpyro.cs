using System;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAIsometricSpyro : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAIsometricSpyro(Context context, GBAIsometric_ObjectType[] types, AnimSet[] animSets) : base(context)
        {
            Types = types;
            AnimSets = animSets;
        }
        
        public GBAIsometric_ObjectType[] Types { get; }
        public AnimSet[] AnimSets { get; }

        public override void InitObjects(Unity_Level level)
        {
            // Init 3D objects
            var all3DObjects = level.EventData.OfType<Unity_Object_GBAIsometricSpyro>().ToArray();

            foreach (var obj in all3DObjects)
            {
                if (obj.Object.ObjectType != 0)
                {
                    GBAIsometricSpyro_ObjInit.GetInitFunc(Context.Settings, obj.ObjType?.Data?.InitFunctionPointer?.AbsoluteOffset ?? 0)?.Invoke(obj, all3DObjects);

                    if (obj.AnimSetIndex == -1 && !obj.IsEditorObj && obj.ObjType?.Data?.InitFunctionPointer != null)
                        Debug.LogWarning($"Type {obj.Object.ObjectType} is not implemented with init function at 0x{obj.ObjType?.Data?.InitFunctionPointer}");
                }
            }

            // Init 2D objects
            var all2DObjects = level.EventData.OfType<Unity_Object_GBAIsometricSpyro2_2D>().ToArray();

            foreach (var obj in all2DObjects)
            {
                // Set link (it only links to door obj array, but since it's the first one this will work)
                if (obj.CanBeLinked && obj.Object.LinkedObjectID != -1)
                {
                    var linkIndex = all2DObjects.FindItemIndex(x => x.Object.ID == obj.Object.LinkedObjectID);

                    if (linkIndex != -1)
                        obj.LinkIndex = linkIndex;
                }

                // Set position
                if (obj.LinkIndex != -1)
                {
                    // If linked we use that position instead
                    obj.XPosition = all2DObjects[obj.LinkIndex].XPosition;
                    obj.YPosition = all2DObjects[obj.LinkIndex].YPosition;
                }
                else
                {
                    obj.XPosition = obj.Object.IsCharacterObj ? obj.Object.MaxX : obj.Object.MinX;
                    obj.YPosition = obj.Object.IsCharacterObj ? obj.Object.MaxY : obj.Object.MinY;
                }

                // Set graphics index based off of type
                if (obj.Object.IsCharacterObj)
                {
                    switch (obj.Object.ObjType)
                    {
                        // Enemies
                        case 0:
                            obj.AnimSetIndex = 0x92;
                            obj.AnimationGroupIndex = 0x05;
                            break;
                        case 2:
                            obj.AnimSetIndex = 0x8A;
                            obj.AnimationGroupIndex = 0x02;
                            break;
                        case 3:
                            obj.AnimSetIndex = 0x84;
                            obj.AnimationGroupIndex = 0x0B;
                            break;

                        // Agent 9
                        case 13:
                            obj.AnimSetIndex = 0x80;
                            obj.AnimationGroupIndex = 0x08;
                            break;

                        case 1:
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                        case 9:
                        case 10:
                        case 11:
                        case 12:
                        case 14:
                        default:
                            obj.AnimSetIndex = -1;
                            break;
                    }
                }
                else
                {
                    switch (obj.Object.ObjType)
                    {
                        // Gem container
                        case 16:
                            obj.AnimSetIndex = 0x7E;
                            obj.AnimationGroupIndex = 0x05;
                            break;

                        default:
                            obj.AnimSetIndex = -1;
                            break;
                    }
                }
            }
        }

        public override string[] LegacyDESNames => Enumerable.Range(0, AnimSets.Length).Select(x => x.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class AnimSet
        {
            public AnimSet(GBAIsometric_Spyro_AnimSet animSetObj, Animation[] animations)
            {
                AnimSetObj = animSetObj;
                Animations = animations;
            }

            public GBAIsometric_Spyro_AnimSet AnimSetObj { get; }
            public Animation[] Animations { get; }

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, byte animSpeed, Vector2Int[] positions)
                {
                    AnimFrameFunc = animFrameFunc;
                    AnimSpeed = animSpeed;
                    Positions = positions;
                }
                 
                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;
                protected Func<Sprite[]> AnimFrameFunc { get; }
                private Vector2Int[] Positions { get; }

                public Sprite[] AnimFrames => Frames ?? (Frames = AnimFrameFunc());

                public Unity_ObjAnimation ObjAnimation => Anim ?? (Anim = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = Positions[x].x,
                            YPosition = Positions[x].y
                        }
                    })).ToArray()
                });
                public byte AnimSpeed { get; }
            }
        }
    }
}