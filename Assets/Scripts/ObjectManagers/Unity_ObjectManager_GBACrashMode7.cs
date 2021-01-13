using System.Linq;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBACrashMode7 : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBACrashMode7(Context context, AnimSet[] animSets) : base(context)
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
                public Animation(Sprite[] sprites)
                {
                    // TODO: Correct animations with different sized frames
                    var pivotX = sprites.Max(x => x.rect.width) / 2;
                    var pivotY = sprites.Max(x => x.rect.height) / 2;

                    AnimFrames = sprites;
                    ObjAnimation = new Unity_ObjAnimation()
                    {
                        Frames = Enumerable.Range(0, AnimFrames.Length).Select(x => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = x,
                                XPosition = 0,
                                YPosition =0 
                            }
                        })).ToArray()
                    };
                }

                public Sprite[] AnimFrames { get; }
                public Unity_ObjAnimation ObjAnimation { get; }
            }
        }
    }
}