using System.Linq;
using BinarySerializer;

using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public class Unity_ObjectManager_GBAIsometricSpyro1_Sparx : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAIsometricSpyro1_Sparx(Context context, AnimSet[] animSets, Sparx_ObjectType[] objectTypes) : base(context)
        {
            AnimSets = animSets;
            ObjectTypes = objectTypes;
        }
        
        public AnimSet[] AnimSets { get; }
        public Sparx_ObjectType[] ObjectTypes { get; }

        public override string[] LegacyDESNames => AnimSets.Select((x, i) => $"{i}").ToArray();
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
                public Animation(Sprite[] frames, Vector2Int offset)
                {
                    Frames = frames;

                    ObjAnimation = new Unity_ObjAnimation()
                    {
                        Frames = Enumerable.Range(0, Frames.Length).Select(i => new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = i,
                                XPosition = offset.x,
                                YPosition = offset.y
                            }
                        })).ToArray()
                    };
                }

                public Sprite[] Frames { get; }
                public Unity_ObjAnimation ObjAnimation { get; }
            }
        }
    }
}