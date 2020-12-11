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
                    GBAIsometric_Spyro_ObjInit.GetInitFunc(Context.Settings, obj.ObjType?.Data?.InitFunctionPointer?.AbsoluteOffset ?? 0)?.Invoke(obj, all3DObjects);

                    if (obj.AnimSetIndex == -1 && !obj.IsEditorObj && obj.ObjType?.Data?.InitFunctionPointer != null)
                        Debug.LogWarning($"Type {obj.Object.ObjectType} is not implemented with init function at 0x{obj.ObjType?.Data?.InitFunctionPointer}");
                }
            }

            // Init 2D objects
            var all2DObjects = level.EventData.OfType<Unity_Object_GBAIsometricSpyro2_2D>().ToArray();

            foreach (var obj in all2DObjects)
            {
                // Set link
                if (obj.CanBeLinked && obj.Object.LinkedObjectID != -1)
                {
                    var linkIndex = all2DObjects.FindItemIndex(x => x.Object.ID == obj.Object.LinkedObjectID && x.Object.Category == GBAIsometric_Spyro2_Object2D.ObjCategory.Door);

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
                    obj.XPosition = obj.Object.MinX;
                    obj.YPosition = obj.Object.MinY;
                }

                // Set graphics index based off of type
                switch (obj.Object.Category)
                {
                    case GBAIsometric_Spyro2_Object2D.ObjCategory.Door:
                        obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x86);

                        switch (Context.Settings.Level)
                        {
                            case 0:
                                obj.AnimationGroupIndex = 0x03;
                                break;
                            case 1:
                                obj.AnimationGroupIndex = 0x05;
                                break;
                            case 2:
                                obj.AnimationGroupIndex = 0x01;
                                break;
                            case 3:
                                obj.AnimationGroupIndex = 0x07;
                                break;
                        }
                        break;

                    case GBAIsometric_Spyro2_Object2D.ObjCategory.Character:
                        switch (obj.Object.ObjType)
                        {
                            // Enemies
                            case 0:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x92);
                                obj.AnimationGroupIndex = 0x05;
                                break;
                            case 1:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x91);
                                obj.AnimationGroupIndex = 0x03;
                                break;
                            case 2:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x8A);
                                obj.AnimationGroupIndex = 0x02;
                                break;
                            case 3:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x84);
                                obj.AnimationGroupIndex = 0x0B;
                                break;
                            case 4:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x82);
                                obj.AnimationGroupIndex = 0x01;
                                break;

                            // No graphics
                            case 5: // Exit
                            case 6:
                            case 10:
                            case 12:
                                obj.IsEditorObj = true;
                                obj.AnimSetIndex = -1;
                                break;

                            // Mine
                            case 7:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x8E);
                                obj.AnimationGroupIndex = 0x00;
                                break;

                            // Floating platforms
                            case 8:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x90);
                                obj.AnimationGroupIndex = 0x02;
                                break;
                            case 9:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x90);
                                obj.AnimationGroupIndex = 0x00;
                                break;

                            // Checkpoint fairy
                            case 11:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x88);
                                obj.AnimationGroupIndex = 0x00;
                                break;

                            // Agent 9
                            case 13:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x80);
                                obj.AnimationGroupIndex = 0x08;
                                break;

                            // Firefly
                            case 14:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x7C);
                                obj.AnimationGroupIndex = 0x03;
                                break;

                            // Sign
                            case 15:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x96);
                                obj.AnimationGroupIndex = 0x00;
                                break;

                            default:
                                obj.AnimSetIndex = -1;
                                break;
                        }
                        break;

                    case GBAIsometric_Spyro2_Object2D.ObjCategory.Collectible:
                        switch (obj.Object.ObjType)
                        {
                            // Gem
                            case 0: // Red
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x89);
                                obj.AnimationGroupIndex = 0x03;
                                break;

                            case 1: // Diamond
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x89);
                                obj.AnimationGroupIndex = 0x00;
                                break;

                            case 2: // Gold
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x89);
                                obj.AnimationGroupIndex = 0x05;
                                break;

                            case 3: // Green
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x89);
                                obj.AnimationGroupIndex = 0x01;
                                break;

                            case 4: // Purple
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x89);
                                obj.AnimationGroupIndex = 0x02;
                                break;

                            // Gem container
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            case 20:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x7E);
                                obj.AnimationGroupIndex = 0x05;
                                break;

                            case 32:
                            case 33:
                            case 34:
                            case 35:
                            case 36:
                                obj.AnimSetIndex = GBAIsometric_Spyro_ObjInit.ConvertAnimSetIndex(Context.Settings.GameModeSelection, 0x7A);
                                obj.AnimationGroupIndex = 0x04;
                                break;

                            default:
                                obj.AnimSetIndex = -1;
                                break;
                        }
                        break;

                    default:
                        obj.AnimSetIndex = -1;
                        break;
                }

                if (obj.AnimSetIndex == -1 && !obj.IsEditorObj)
                    Debug.LogWarning($"Type {obj.Object.ObjType} with category {obj.Object.Category} has no graphics");
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