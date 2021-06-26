using BinarySerializer;
using System;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAKlonoa : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAKlonoa(Context context, AnimSet[] animSets) : base(context)
        {
            AnimSets = animSets;
        }

        public AnimSet[] AnimSets { get; }

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
                public Animation(Func<Sprite[]> animFrameFunc)
                {
                    AnimFrameFunc = animFrameFunc;
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;
                protected Func<Sprite[]> AnimFrameFunc { get; }

                public Sprite[] AnimFrames => Frames ??= AnimFrameFunc();

                public Unity_ObjAnimation ObjAnimation => Anim ??= new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x =>
                    {
                        return new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = x
                            }
                        });
                    }).ToArray()
                };
            }
        }
    }
}