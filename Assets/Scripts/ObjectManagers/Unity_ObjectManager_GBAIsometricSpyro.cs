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
            var allObjects = level.EventData.Cast<Unity_Object_GBAIsometricSpyro>().ToArray();

            foreach (var obj in allObjects)
                GBAIsometricSpyro_ObjInit.GetInitFunc(Context.Settings, obj.ObjType?.Data?.InitFunctionPointer?.AbsoluteOffset ?? 0)?.Invoke(obj, allObjects);
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