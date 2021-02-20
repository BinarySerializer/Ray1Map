using System;
using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAVV : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAVV(Context context, AnimSet[] animSets, GBAVV_Map2D_ObjData objData, GBAVV_MapInfo.GBAVV_MapType mapType) : base(context)
        {
            AnimSets = animSets;
            ObjData = objData;
            MapType = mapType;
        }
        
        public AnimSet[] AnimSets { get; }
        public GBAVV_Map2D_ObjData ObjData { get; }
        public GBAVV_MapInfo.GBAVV_MapType MapType { get; }
        public byte[][] ObjParams => ObjData.ObjParams;

        public override string[] LegacyDESNames => AnimSets.Select((x, i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class AnimSet
        {
            public AnimSet(Animation[] animations)
            {
                Animations = animations;
            }

            public Animation[] Animations { get; }

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, GBAVV_Map2D_Animation crashAnim, int xPos, int yPos)
                {
                    AnimFrameFunc = animFrameFunc;
                    CrashAnim = crashAnim;
                    XPos = xPos;
                    YPos = yPos;

                    AnimHitBox = CrashAnim.HitBox == null ? new Unity_ObjAnimationCollisionPart[0] : new Unity_ObjAnimationCollisionPart[]
                    {
                        new Unity_ObjAnimationCollisionPart()
                        {
                            Type = Unity_ObjAnimationCollisionPart.CollisionType.HitTriggerBox,
                            XPosition = CrashAnim.HitBox.X,
                            YPosition = CrashAnim.HitBox.Y,
                            Width = CrashAnim.HitBox.Width + 1,
                            Height = CrashAnim.HitBox.Height + 1,
                        },
                    };
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;
                protected Func<Sprite[]> AnimFrameFunc { get; }
                public GBAVV_Map2D_Animation CrashAnim { get; }
                public int XPos { get; }
                public int YPos { get; }

                public Sprite[] AnimFrames => Frames ?? (Frames = AnimFrameFunc());

                public Unity_ObjAnimation ObjAnimation => Anim ?? (Anim = new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                    {
                        new Unity_ObjAnimationPart()
                        {
                            ImageIndex = x,
                            XPosition = XPos,
                            YPosition = YPos
                        }
                    })).ToArray()
                });

                public Unity_ObjAnimationCollisionPart[] AnimHitBox { get; } 
            }
        }
    }
}