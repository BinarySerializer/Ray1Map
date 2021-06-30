using BinarySerializer;
using System;
using System.Collections.Generic;
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

        public override string[] LegacyDESNames => AnimSets.Select(x => x.DisplayName).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class AnimSet
        {
            public AnimSet(Animation[] animations, string displayName, IEnumerable<GBAKlonoa_ObjectOAMCollection> oamCollections)
            {
                Animations = animations;
                DisplayName = displayName;
                OAMCollections = new List<GBAKlonoa_ObjectOAMCollection>(oamCollections);
            }

            public Animation[] Animations { get; }
            public List<GBAKlonoa_ObjectOAMCollection> OAMCollections { get; }
            public string DisplayName { get; }

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, GBAKlonoa_ObjectOAMCollection oamCollection)
                {
                    AnimFrameFunc = animFrameFunc;
                    OAMCollection = oamCollection;
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;

                protected Func<Sprite[]> AnimFrameFunc { get; }
                public GBAKlonoa_ObjectOAMCollection OAMCollection { get; }

                public Sprite[] AnimFrames => Frames ??= AnimFrameFunc();

                public Unity_ObjAnimation ObjAnimation => Anim ??= new Unity_ObjAnimation()
                {
                    Frames = Enumerable.Range(0, AnimFrames.Length).Select(x =>
                    {
                        return new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                            {
                                ImageIndex = x,
                                XPosition = OAMCollection.OAMs[0].XPos,
                                YPosition = OAMCollection.OAMs[0].YPos,
                            }
                        });
                    }).ToArray()
                };
            }
        }
    }
}