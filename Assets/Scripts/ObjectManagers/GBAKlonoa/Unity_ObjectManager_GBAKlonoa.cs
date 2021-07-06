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
            public AnimSet(Animation[] animations, string displayName, IEnumerable<GBAKlonoa_OAM[]> oamCollections, int dct_GraphisIndex = -1)
            {
                Animations = animations;
                DisplayName = displayName;
                DCT_GraphisIndex = dct_GraphisIndex;
                OAMCollections = new List<GBAKlonoa_OAM[]>(oamCollections);
            }

            public Animation[] Animations { get; }
            public List<GBAKlonoa_OAM[]> OAMCollections { get; }
            public string DisplayName { get; }
            public int DCT_GraphisIndex { get; }

            public class Animation
            {
                public Animation(Func<Sprite[]> animFrameFunc, GBAKlonoa_OAM[] oamCollection, int?[] animSpeeds = null)
                {
                    AnimFrameFunc = animFrameFunc;
                    OAMCollection = oamCollection;
                    AnimSpeeds = animSpeeds;
                }

                private Sprite[] Frames;
                private Unity_ObjAnimation Anim;

                protected Func<Sprite[]> AnimFrameFunc { get; }
                public GBAKlonoa_OAM[] OAMCollection { get; }
                public int?[] AnimSpeeds { get; }

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
                                XPosition = OAMCollection[0].XPos,
                                YPosition = OAMCollection[0].YPos,
                            }
                        });
                    }).ToArray()
                };
            }
        }
    }
}